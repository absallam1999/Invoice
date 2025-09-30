using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using invoice.Repo.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class PayInvoiceService : IPayInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IRepository<PayInvoice> _payInvoiceRepo;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;

        public PayInvoiceService(
            ApplicationDbContext dbContext,
            IPaymentGatewayService paymentGatewayService,
            IPaymentMethodService paymentMethodService,
            IRepository<PayInvoice> payInvoiceRepo,
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo)
        {
            _paymentGatewayService = paymentGatewayService;
            _paymentMethodService = paymentMethodService;
            _payInvoiceRepo = payInvoiceRepo;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _dbContext = dbContext;
        }

        private static bool IsEmpty(string value) => string.IsNullOrWhiteSpace(value);

        private static void StampNew(PayInvoice invoice) => invoice.UpdatedAt = DateTime.UtcNow;

        private static void StampUpdate(PayInvoice invoice) =>
            invoice.UpdatedAt = DateTime.UtcNow;

        private static PaymentStatusResponse ToStatusResponse(PayInvoice p) => new()
        {
            PaymentId = p.Id,
            Status = p.Status,
            LastUpdated = p.UpdatedAt ?? DateTime.UtcNow,
            Amount = p.Amount,
            Currency = p.Currency
        };

        private async Task<PayInvoice> QueryFirstOrDefaultAsync(Expression<Func<PayInvoice, bool>> predicate)
        {
            var items = await _payInvoiceRepo.QueryAsync(predicate);
            return items.FirstOrDefault();
        }

        private static string ExtractSessionIdFromUrl(string paymentUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentUrl)) return null;
                var uri = new Uri(paymentUrl);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                return query["session_id"] ?? query["token"] ?? query["id"];
            }
            catch { return null; }
        }

        public async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(
            string invoiceId,
            PaymentType paymentType,
            string userId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice ID is required");

            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice not found or access denied");

            if (invoice.InvoiceStatus == InvoiceStatus.Paid)
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice already paid");

            if (invoice.InvoiceStatus is InvoiceStatus.Cancelled or InvoiceStatus.Refunded)
                return new GeneralResponse<PaymentSessionResponse>(false, $"Cannot process payment for {invoice.InvoiceStatus} invoice");

            var existing = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Pending);
            if (existing.Any())
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice already has a pending payment");

            var paymentDto = new PaymentCreateDTO
            {
                Name = $"Invoice Payment - {invoice.Code}",
                Cost = invoice.FinalValue,
                InvoiceId = invoiceId,
                Description = $"Payment for invoice {invoice.Code}",
                ClientId = invoice.ClientId,
                ClientEmail = invoice.Client?.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "invoice_code", invoice.Code },
                    { "client_id", invoice.ClientId },
                    { "created_date", invoice.CreatedAt.ToString("O") }
                }
            };

            var sessionResponse = await _paymentGatewayService.CreatePaymentSessionAsync(paymentDto, paymentType);
            if (!sessionResponse.Success)
                return sessionResponse;

            var sessionId = sessionResponse.Data?.SessionId ?? ExtractSessionIdFromUrl(sessionResponse.Data?.PaymentUrl);
            if (string.IsNullOrWhiteSpace(sessionId))
                return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create payment session: No session ID returned");

            var methodId = await _paymentMethodService.GetIdFromTypeAsync(paymentType);

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    var payment = new Payment
                    {
                        Name = $"Invoice Payment - {invoice.Code}",
                        Link = sessionResponse.Data.PaymentUrl,
                        GatewaySessionId = sessionId,
                        Cost = invoice.FinalValue,
                        Currency = "USD",
                        ExpiresAt = DateTime.UtcNow.AddDays(3),
                        Status = PaymentStatus.Pending,
                        UserId = userId,
                        InvoiceId = invoice.Id,
                        PaymentMethodId = methodId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };


                    var paymentResult = await _paymentRepo.AddAsync(payment);
                    if (!paymentResult.Success)
                    {
                        await tx.RollbackAsync();
                        return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create payment record");
                    }

                    invoice.Payments ??= new();
                    invoice.Payments.Add(payment);

                    var invoiceUpdate = await _invoiceRepo.UpdateAsync(invoice);
                    if (!invoiceUpdate.Success)
                    {
                        await tx.RollbackAsync();
                        return new GeneralResponse<PaymentSessionResponse>(false, "Failed to update invoice with new payment");
                    }

                    var payInvoice = new PayInvoice
                    {
                        InvoiceId = invoice.Id,
                        PaymentMethodId = methodId,
                        Status = PaymentStatus.Pending,
                        PaymentUrl = sessionResponse.Data.PaymentUrl,
                        PaymentSessionId = sessionId,
                        PaymentGatewayType = paymentType,
                        PaymentGatewayResponse = sessionResponse.Data?.RawResponse ?? "{}",
                        Amount = invoice.FinalValue,
                    };

                    StampNew(payInvoice);
                    var payInvoiceResult = await _payInvoiceRepo.AddAsync(payInvoice);
                    if (!payInvoiceResult.Success)
                    {
                        await tx.RollbackAsync();
                        return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create gateway payment record");
                    }

                    await tx.CommitAsync();
                    return sessionResponse;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();

                    return new GeneralResponse<PaymentSessionResponse>(
                        false,
                        $"Error creating payment session: {ex.Message}"
                    );
                }

            });
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            if (IsEmpty(paymentId))
                return new GeneralResponse<PaymentStatusResponse>(false, "Payment ID is required");

            var payInvoice = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payInvoice == null)
                return new GeneralResponse<PaymentStatusResponse>(false, "Payment not found");

            return new GeneralResponse<PaymentStatusResponse>(true, null, ToStatusResponse(payInvoice));
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusBySessionAsync(string sessionId, PaymentType paymentType)
        {
            if (IsEmpty(sessionId))
                return new GeneralResponse<PaymentStatusResponse>(false, "Session ID is required");

            var payInvoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId && p.PaymentGatewayType == paymentType);
            if (payInvoice == null)
                return new GeneralResponse<PaymentStatusResponse>(false, "Payment not found");

            return new GeneralResponse<PaymentStatusResponse>(true, null, ToStatusResponse(payInvoice));
        }

        public async Task<GeneralResponse<IEnumerable<PaymentStatusResponse>>> GetPaymentStatusesByInvoiceAsync(string invoiceId)
        {
            if (IsEmpty(invoiceId))
                return new GeneralResponse<IEnumerable<PaymentStatusResponse>>(false, "Invoice ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId);
            var statuses = payments.Select(ToStatusResponse);

            return new GeneralResponse<IEnumerable<PaymentStatusResponse>>(true, null, statuses);
        }

        public async Task<GeneralResponse<PayInvoice>> CreateAsync(PayInvoice payInvoice)
        {
            if (payInvoice == null)
                return new GeneralResponse<PayInvoice>(false, "Payment data is required");

            StampNew(payInvoice);
            return await _payInvoiceRepo.AddAsync(payInvoice);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> CreateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            if (payInvoices == null || !payInvoices.Any())
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "Payment data is required");

            foreach (var invoice in payInvoices) StampNew(invoice);
            return await _payInvoiceRepo.AddRangeAsync(payInvoices);
        }

        public async Task<GeneralResponse<PayInvoice>> UpdateAsync(string id, PayInvoice payInvoice)
        {
            if (IsEmpty(id))
                return new GeneralResponse<PayInvoice>(false, "ID is required");
            if (payInvoice == null)
                return new GeneralResponse<PayInvoice>(false, "Payment data is required");

            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            existing.Status = payInvoice.Status;
            existing.Amount = payInvoice.Amount;
            existing.Currency = payInvoice.Currency;
            existing.PaymentGatewayResponse = payInvoice.PaymentGatewayResponse;
            StampUpdate(existing);

            return await _payInvoiceRepo.UpdateAsync(existing);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            if (payInvoices == null || !payInvoices.Any())
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "No payments provided");

            foreach (var invoice in payInvoices)
                StampUpdate(invoice);

            return await _payInvoiceRepo.UpdateRangeAsync(payInvoices);
        }

        public async Task<GeneralResponse<PayInvoice>> UpdateStatusAsync(string id, PaymentStatus status, string callbackData = null)
        {
            if (IsEmpty(id))
                return new GeneralResponse<PayInvoice>(false, "ID is required");

            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            existing.Status = status;
            existing.PaymentGatewayResponse = callbackData;
            StampUpdate(existing);

            if (status == PaymentStatus.Completed) existing.PaidAt = DateTime.UtcNow;
            else if (status == PaymentStatus.Refunded) existing.RefundedAt = DateTime.UtcNow;

            return await _payInvoiceRepo.UpdateAsync(existing);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (IsEmpty(id))
                return new GeneralResponse<bool>(false, "ID is required");

            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<bool>(false, "Payment not found");

            var result = await _payInvoiceRepo.DeleteAsync(existing.Id);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
                return new GeneralResponse<bool>(false, "No IDs provided");

            var invoices = await _payInvoiceRepo.QueryAsync(p => ids.Contains(p.Id));
            if (!invoices.Any())
                return new GeneralResponse<bool>(false, "No payments found");

            var Ids = new List<string>();
            foreach (var invoice in invoices) {
                Ids.Add(invoice.Id);
            }
            var result = await _payInvoiceRepo.DeleteRangeAsync(Ids);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }

        public async Task<GeneralResponse<bool>> DeleteBySessionIdAsync(string sessionId)
        {
            if (IsEmpty(sessionId))
                return new GeneralResponse<bool>(false, "Session ID is required");

            var payInvoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            if (payInvoice == null)
                return new GeneralResponse<bool>(false, "Payment not found");

            var result = await _payInvoiceRepo.DeleteAsync(payInvoice.Id);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }


        public async Task<GeneralResponse<bool>> RetryFailedPaymentAsync(string paymentId)
        {
            if (IsEmpty(paymentId))
                return new GeneralResponse<bool>(false, "Payment ID is required");

            var payInvoice = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payInvoice == null)
                return new GeneralResponse<bool>(false, "Payment not found");

            if (payInvoice.Status != PaymentStatus.Failed)
                return new GeneralResponse<bool>(false, "Only failed payments can be retried");

            payInvoice.Status = PaymentStatus.Pending;
            StampUpdate(payInvoice);

            var updateResult = await _payInvoiceRepo.UpdateAsync(payInvoice);
            return new GeneralResponse<bool>(updateResult.Success, updateResult.Message, updateResult.Success);
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidAmountAsync(string invoiceId)
        {
            if (IsEmpty(invoiceId))
                return new GeneralResponse<decimal>(false, "Invoice ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Completed);
            var total = payments.Sum(p => p.Amount);

            return new GeneralResponse<decimal>(true, null, total);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId)
        {
            var payments = string.IsNullOrEmpty(userId)
                ? await _payInvoiceRepo.GetAllAsync()
                : await _payInvoiceRepo.QueryAsync(p => p.Invoice.UserId == userId);

            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }

        public async Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId)
        {
            if (IsEmpty(id))
                return new GeneralResponse<PayInvoice>(false, "Payment ID is required");

            var payment = await _payInvoiceRepo.GetByIdAsync(id);

            if (payment == null)
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            if (payment.Invoice == null && !string.IsNullOrEmpty(payment.InvoiceId))
            {
                var invoice = await _invoiceRepo.GetByIdAsync(payment.InvoiceId);
                payment.Invoice = invoice;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                if (payment.Invoice == null || payment.Invoice.UserId != userId)
                    return new GeneralResponse<PayInvoice>(false, "Payment not found or access denied");
            }

            return new GeneralResponse<PayInvoice>(true, null, payment);
        }


        public async Task<GeneralResponse<PayInvoice>> GetBySessionIdAsync(string sessionId, string userId)
        {
            if (IsEmpty(sessionId))
                return new GeneralResponse<PayInvoice>(false, "Session ID is required");

            var payment = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            if (payment == null || (!string.IsNullOrEmpty(userId) && payment.Invoice.UserId != userId))
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            return new GeneralResponse<PayInvoice>(true, null, payment);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId)
        {
            if (IsEmpty(invoiceId))
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "Invoice ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId && (string.IsNullOrEmpty(userId) || p.Invoice.UserId == userId));
            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId)
        {
            if (IsEmpty(paymentMethodId))
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "Payment Method ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (string.IsNullOrEmpty(userId) || p.Invoice.UserId == userId));
            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }
        public async Task<GeneralResponse<PayInvoice>> GetByPaymentIdAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<PayInvoice>(false, "Payment ID is required");

            var payInvoice = await _payInvoiceRepo.GetQueryable()
                .Include(p => p.Invoice).ThenInclude(i => i.Payments)
                .FirstOrDefaultAsync(p => p.Id == paymentId && (userId == null || p.Invoice.UserId == userId));

            if (payInvoice == null)
                return new GeneralResponse<PayInvoice>(false, "PayInvoice not found");

            return new GeneralResponse<PayInvoice>(true, "PayInvoice retrieved successfully", payInvoice);
        }
    }
}