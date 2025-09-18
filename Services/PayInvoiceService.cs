using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entites;
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
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IRepository<PayInvoice> _payInvoiceRepo;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public PayInvoiceService(
            ApplicationDbContext dbContext,
            IPaymentGatewayService paymentGatewayService,
            IPaymentMethodService paymentMethodService,
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<PayInvoice> payInvoiceRepo,
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo,
            IInvoiceService invoiceService,
            IMapper mapper)
        {
            _paymentGatewayService = paymentGatewayService;
            _paymentMethodService = paymentMethodService;
            _paymentLinkRepo = paymentLinkRepo;
            _payInvoiceRepo = payInvoiceRepo;
            _invoiceService = invoiceService;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private static bool IsEmpty(string value) => string.IsNullOrWhiteSpace(value);

        private static void StampNew(PayInvoice invoice)
        {
            var now = DateTime.UtcNow;
            invoice.CreatedAt = now;
            invoice.UpdatedAt = now;
        }

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
            string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice ID is required");

            var invoiceResponse = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoiceResponse == null)
                return new GeneralResponse<PaymentSessionResponse>(false, "Invoice not found or access denied");

            var invoice = invoiceResponse;

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
                Currency = invoice.Currency ?? "USD",
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

            using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var paymentLink = new PaymentLink
                {
                    Link = sessionResponse.Data.PaymentUrl,
                    GatewaySessionId = sessionId,
                    Value = invoice.FinalValue,
                    Currency = invoice.Currency ?? "USD",
                    Purpose = $"Payment for invoice {invoice.Code}",
                    PaymentsNumber = "1",
                    Description = $"Invoice {invoice.Code} payment",
                    Message = "Please complete your payment",
                    Image = string.Empty,
                    Terms = "Standard terms",
                    CreatedBy = userId,
                    InvoiceId = invoice.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var linkResult = await _paymentLinkRepo.AddAsync(paymentLink);
                if (!linkResult.Success)
                {
                    await tx.RollbackAsync();
                    return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create payment link");
                }

                var payment = new Payment
                {
                    Name = $"Invoice Payment - {invoice.Code}",
                    Cost = invoice.FinalValue,
                    Status = PaymentStatus.Pending,
                    UserId = userId,
                    InvoiceId = invoice.Id,
                    PaymentMethodId = methodId,
                    PaymentLink = paymentLink,
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
                    PaymentSessionId = sessionId,
                    PaymentGatewayType = paymentType,
                    PaymentGatewayResponse = sessionResponse.Data?.RawResponse ?? "{}",
                    Amount = invoice.FinalValue,
                    Currency = invoice.Currency ?? "USD"
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
                return new GeneralResponse<PaymentSessionResponse>(false, $"Error creating payment session: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<bool>> ProcessPaymentCallbackAsync(string paymentSessionId, PaymentType paymentType, bool isSuccess, string callbackData = null)
        {
            if (IsEmpty(paymentSessionId))
                return new GeneralResponse<bool>(false, "Payment session ID is required");

            var payInvoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == paymentSessionId && p.PaymentGatewayType == paymentType);
            if (payInvoice == null)
                return new GeneralResponse<bool>(false, "Payment record not found");

            if (payInvoice.Status != PaymentStatus.Pending)
                return new GeneralResponse<bool>(true, $"Already processed: {payInvoice.Status}", true);

            payInvoice.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
            payInvoice.PaidAt = isSuccess ? DateTime.UtcNow : payInvoice.PaidAt;
            payInvoice.PaymentGatewayResponse = callbackData;
            StampUpdate(payInvoice);

            var updateResult = await _payInvoiceRepo.UpdateAsync(payInvoice);
            if (!updateResult.Success)
                return new GeneralResponse<bool>(false, "Failed to update payment record");

            if (isSuccess)
            {
                var statusUpdate = await _invoiceService.UpdateStatusAsync(payInvoice.InvoiceId, InvoiceStatus.Paid, payInvoice.Id);
                if (!statusUpdate.Success)
                    return new GeneralResponse<bool>(false, "Payment processed but failed to update invoice status");
            }

            return new GeneralResponse<bool>(true, "Payment processed successfully", true);
        }

        public async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType, decimal? amount = null)
        {
            if (IsEmpty(paymentId))
                return new GeneralResponse<bool>(false, "Payment ID is required");

            var payInvoice = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payInvoice == null)
                return new GeneralResponse<bool>(false, "Payment not found");

            if (payInvoice.Status != PaymentStatus.Completed)
                return new GeneralResponse<bool>(false, "Only completed payments can be refunded");

            var refundAmount = amount ?? payInvoice.Amount;
            if (refundAmount <= 0 || refundAmount > payInvoice.Amount)
                return new GeneralResponse<bool>(false, "Invalid refund amount");

            var refundResponse = await _paymentGatewayService.RefundPaymentAsync(payInvoice.PaymentSessionId, paymentType);
            if (!refundResponse.Success) return refundResponse;

            payInvoice.Status = PaymentStatus.Refunded;
            payInvoice.RefundAmount = refundAmount;
            payInvoice.RefundedAt = DateTime.UtcNow;
            StampUpdate(payInvoice);

            var updateResult = await _payInvoiceRepo.UpdateAsync(payInvoice);
            if (!updateResult.Success)
                return new GeneralResponse<bool>(false, "Failed to update payment record");

            var statusUpdate = await _invoiceService.UpdateStatusAsync(payInvoice.InvoiceId, InvoiceStatus.Refunded, payInvoice.Id);
            if (!statusUpdate.Success)
                return new GeneralResponse<bool>(false, "Refund processed but failed to update invoice status");

            return new GeneralResponse<bool>(true, "Refund successful", true);
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            if (IsEmpty(paymentId))
                return new GeneralResponse<bool>(false, "Payment ID is required");

            var payment = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payment == null)
                return new GeneralResponse<bool>(false, "Payment not found");

            if (payment.Status != PaymentStatus.Pending)
                return new GeneralResponse<bool>(false, "Only pending payments can be cancelled");

            payment.Status = PaymentStatus.Cancelled;
            StampUpdate(payment);

            var updateResult = await _payInvoiceRepo.UpdateAsync(payment);
            if (!updateResult.Success)
                return new GeneralResponse<bool>(false, "Failed to cancel payment");

            return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
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

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            if (payInvoices == null || !payInvoices.Any())
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "No payments provided");

            foreach (var invoice in payInvoices)
                StampUpdate(invoice);

            return await _payInvoiceRepo.UpdateRangeAsync(payInvoices);
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

        public async Task<bool> ExistsAsync(string id)
        {
            if (IsEmpty(id)) return false;
            var invoice = await _payInvoiceRepo.GetByIdAsync(id);
            return invoice != null;
        }

        public async Task<bool> ExistsBySessionAsync(string sessionId)
        {
            if (IsEmpty(sessionId)) return false;
            var invoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            return invoice != null;
        }

        public async Task<int> CountAsync(string invoiceId = null)
        {
            if (IsEmpty(invoiceId)) return await _payInvoiceRepo.CountAsync();
            return await _payInvoiceRepo.CountAsync(p => p.InvoiceId == invoiceId);
        }

        public async Task<int> CountByStatusAsync(PaymentStatus status, string invoiceId = null)
        {
            if (IsEmpty(invoiceId))
                return await _payInvoiceRepo.CountAsync(p => p.Status == status);
            return await _payInvoiceRepo.CountAsync(p => p.Status == status && p.InvoiceId == invoiceId);
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

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId = null)
        {
            var payments = string.IsNullOrEmpty(userId)
                ? await _payInvoiceRepo.GetAllAsync()
                : await _payInvoiceRepo.QueryAsync(p => p.Invoice.UserId == userId);

            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }

        public async Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId = null)
        {
            if (IsEmpty(id))
                return new GeneralResponse<PayInvoice>(false, "Payment ID is required");

            var payment = await _payInvoiceRepo.GetByIdAsync(id);
            if (payment == null || (!string.IsNullOrEmpty(userId) && payment.Invoice.UserId != userId))
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            return new GeneralResponse<PayInvoice>(true, null, payment);
        }

        public async Task<GeneralResponse<PayInvoice>> GetBySessionIdAsync(string sessionId, string userId = null)
        {
            if (IsEmpty(sessionId))
                return new GeneralResponse<PayInvoice>(false, "Session ID is required");

            var payment = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            if (payment == null || (!string.IsNullOrEmpty(userId) && payment.Invoice.UserId != userId))
                return new GeneralResponse<PayInvoice>(false, "Payment not found");

            return new GeneralResponse<PayInvoice>(true, null, payment);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            if (IsEmpty(invoiceId))
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "Invoice ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId && (string.IsNullOrEmpty(userId) || p.Invoice.UserId == userId));
            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null)
        {
            if (IsEmpty(paymentMethodId))
                return new GeneralResponse<IEnumerable<PayInvoice>>(false, "Payment Method ID is required");

            var payments = await _payInvoiceRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (string.IsNullOrEmpty(userId) || p.Invoice.UserId == userId));
            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByStatusAsync(PaymentStatus status, string userId = null)
        {
            var payments = await _payInvoiceRepo.QueryAsync(p => p.Status == status && (string.IsNullOrEmpty(userId) || p.Invoice.UserId == userId));
            return new GeneralResponse<IEnumerable<PayInvoice>>(true, null, payments);
        }
    }
}