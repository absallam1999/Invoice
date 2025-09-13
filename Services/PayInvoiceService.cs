using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class PayInvoiceService : IPayInvoiceService
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IRepository<PayInvoice> _payInvoiceRepo;
        private readonly IInvoiceService _invoiceService;

        public PayInvoiceService(
            IPaymentGatewayService paymentGatewayService,
            IRepository<PayInvoice> payInvoiceRepo,
            IInvoiceService invoiceService)
        {
            _paymentGatewayService = paymentGatewayService;
            _payInvoiceRepo = payInvoiceRepo;
            _invoiceService = invoiceService;
        }

        public async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(string invoiceId, PaymentType paymentType, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = "Invoice ID is required" };

            var invoiceResponse = await _invoiceService.GetByIdAsync(invoiceId, userId);
            if (!invoiceResponse.Success || invoiceResponse.Data == null)
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = "Invoice not found or access denied" };

            var invoice = invoiceResponse.Data;

            if (invoice.InvoiceStatus == InvoiceStatus.Paid)
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = "Invoice already paid" };

            if (invoice.InvoiceStatus == InvoiceStatus.Cancelled || invoice.InvoiceStatus == InvoiceStatus.Refunded)
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = $"Cannot process payment for invoice with status {invoice.InvoiceStatus}" };

            var existing = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Pending);
            if (existing.Any())
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = "Invoice already has a pending payment" };

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
            if (!sessionResponse.Success) return sessionResponse;

            var sessionId = sessionResponse.Data?.SessionId ?? ExtractSessionIdFromUrl(sessionResponse.Data?.PaymentUrl) ?? Guid.NewGuid().ToString();

            var payInvoice = new PayInvoice
            {
                InvoiceId = invoiceId,
                PaymentMethodId = GetPaymentMethodIdFromType(paymentType),
                Status = PaymentStatus.Pending,
                PaymentSessionId = sessionId,
                PaymentGatewayType = paymentType,
                Amount = invoice.FinalValue,
                Currency = invoice.Currency ?? "USD",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var addResult = await _payInvoiceRepo.AddAsync(payInvoice);
            if (!addResult.Success)
                return new GeneralResponse<PaymentSessionResponse> { Success = false, Message = "Failed to create payment record" };

            return sessionResponse;
        }

        public async Task<GeneralResponse<bool>> ProcessPaymentCallbackAsync(string paymentSessionId, PaymentType paymentType, bool isSuccess, string callbackData = null)
        {
            if (string.IsNullOrWhiteSpace(paymentSessionId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment session ID is required" };

            var payInvoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == paymentSessionId && p.PaymentGatewayType == paymentType);
            if (payInvoice == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment record not found" };

            if (payInvoice.Status != PaymentStatus.Pending)
                return new GeneralResponse<bool> { Success = true, Message = $"Already processed: {payInvoice.Status}", Data = true };

            payInvoice.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
            payInvoice.PaidAt = isSuccess ? DateTime.UtcNow : payInvoice.PaidAt; 
            payInvoice.PaymentGatewayResponse = callbackData;
            payInvoice.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _payInvoiceRepo.UpdateAsync(payInvoice);
            if (!updateResult.Success)
                return new GeneralResponse<bool> { Success = false, Message = "Failed to update payment record" };

            if (isSuccess)
            {
                var statusUpdateResult = await _invoiceService.UpdateStatusAsync(payInvoice.InvoiceId, InvoiceStatus.Paid, payInvoice.Id);
                if (!statusUpdateResult.Success)
                    return new GeneralResponse<bool> { Success = false, Message = "Payment processed but failed to update invoice status" };
            }

            return new GeneralResponse<bool> { Success = true, Message = "Payment processed successfully", Data = true };
        }

        public async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType, decimal? amount = null)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payInvoice = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payInvoice == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payInvoice.Status != PaymentStatus.Completed)
                return new GeneralResponse<bool> { Success = false, Message = "Only completed payments can be refunded" };

            var refundAmount = amount ?? payInvoice.Amount;
            if (refundAmount <= 0 || refundAmount > payInvoice.Amount)
                return new GeneralResponse<bool> { Success = false, Message = "Invalid refund amount" };

            var refundResponse = await _paymentGatewayService.RefundPaymentAsync(payInvoice.PaymentSessionId, paymentType);
            if (!refundResponse.Success) return refundResponse;

            payInvoice.Status = PaymentStatus.Refunded;
            payInvoice.RefundAmount = refundAmount;
            payInvoice.RefundedAt = DateTime.UtcNow;
            payInvoice.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _payInvoiceRepo.UpdateAsync(payInvoice);
            if (!updateResult.Success)
                return new GeneralResponse<bool> { Success = false, Message = "Failed to update payment record" };

            var statusUpdateResult = await _invoiceService.UpdateStatusAsync(payInvoice.InvoiceId, InvoiceStatus.Refunded, payInvoice.Id);
            if (!statusUpdateResult.Success)
                return new GeneralResponse<bool> { Success = false, Message = "Refund processed but failed to update invoice status" };

            return new GeneralResponse<bool> { Success = true, Message = "Refund successful", Data = true };
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<PaymentStatusResponse> { Success = false, Message = "Payment ID is required" };

            var payInvoice = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payInvoice == null)
                return new GeneralResponse<PaymentStatusResponse> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PaymentStatusResponse>
            {
                Success = true,
                Data = new PaymentStatusResponse
                {
                    PaymentId = payInvoice.Id,
                    Status = payInvoice.Status,
                    LastUpdated = (DateTime)payInvoice.UpdatedAt,
                    Amount = payInvoice.Amount,
                    Currency = payInvoice.Currency
                }
            };
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusBySessionAsync(string sessionId, PaymentType paymentType)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return new GeneralResponse<PaymentStatusResponse> { Success = false, Message = "Session ID is required" };

            var payInvoice = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId && p.PaymentGatewayType == paymentType);
            if (payInvoice == null)
                return new GeneralResponse<PaymentStatusResponse> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PaymentStatusResponse>
            {
                Success = true,
                Data = new PaymentStatusResponse
                {
                    PaymentId = payInvoice.Id,
                    Status = payInvoice.Status,
                    LastUpdated = (DateTime)payInvoice.UpdatedAt,
                    Amount = payInvoice.Amount,
                    Currency = payInvoice.Currency
                }
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentStatusResponse>>> GetPaymentStatusesByInvoiceAsync(string invoiceId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<IEnumerable<PaymentStatusResponse>> { Success = false, Message = "Invoice ID is required" };

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId);
            var statusResponses = payments.Select(p => new PaymentStatusResponse
            {
                PaymentId = p.Id,
                Status = p.Status,
                LastUpdated = (DateTime)p.UpdatedAt,
                Amount = p.Amount,
                Currency = p.Currency
            });

            return new GeneralResponse<IEnumerable<PaymentStatusResponse>>
            {
                Success = true,
                Data = statusResponses
            };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId = null)
        {
            var payments = await _payInvoiceRepo.GetAllAsync();
            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Data = payments };
        }

        public async Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PayInvoice> { Success = false, Message = "ID is required" };

            var payment = await _payInvoiceRepo.GetByIdAsync(id);
            if (payment == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PayInvoice> { Success = true, Data = payment };
        }

        public async Task<GeneralResponse<PayInvoice>> GetBySessionIdAsync(string sessionId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Session ID is required" };

            var payment = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            if (payment == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PayInvoice> { Success = true, Data = payment };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Invoice ID is required" };

            var payments = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId);
            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Data = payments };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Payment method ID is required" };

            var payments = await _payInvoiceRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId);
            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Data = payments };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByStatusAsync(PaymentStatus status, string userId = null)
        {
            var payments = await _payInvoiceRepo.QueryAsync(p => p.Status == status);
            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Data = payments };
        }

        public async Task<GeneralResponse<PayInvoice>> CreateAsync(PayInvoice payInvoice)
        {
            if (payInvoice == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment data is required" };
            
            var now = DateTime.UtcNow;
            payInvoice.CreatedAt = now;
            payInvoice.UpdatedAt = now;

            return await _payInvoiceRepo.AddAsync(payInvoice);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> CreateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            if (payInvoices == null || !payInvoices.Any())
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Payment data is required" };

            var now = DateTime.UtcNow;
            foreach (var invoice in payInvoices)
            {
                invoice.CreatedAt = now;
                invoice.UpdatedAt = now;
            }

            return await _payInvoiceRepo.AddRangeAsync(payInvoices);
        }

        public async Task<GeneralResponse<PayInvoice>> UpdateAsync(string id, PayInvoice payInvoice)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PayInvoice> { Success = false, Message = "ID is required" };

            if (payInvoice == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment data is required" };

            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment not found" };

            existing.Status = payInvoice.Status;
            existing.Amount = payInvoice.Amount;
            existing.Currency = payInvoice.Currency;
            existing.PaymentGatewayResponse = payInvoice.PaymentGatewayResponse;
            existing.UpdatedAt = DateTime.UtcNow;

            return await _payInvoiceRepo.UpdateAsync(existing);
        }

        public async Task<GeneralResponse<PayInvoice>> UpdateStatusAsync(string id, PaymentStatus status, string callbackData = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PayInvoice> { Success = false, Message = "ID is required" };

            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Payment not found" };

            existing.Status = status;
            existing.PaymentGatewayResponse = callbackData;
            existing.UpdatedAt = DateTime.UtcNow;

            if (status == PaymentStatus.Completed)
                existing.PaidAt = DateTime.UtcNow;
            else if (status == PaymentStatus.Refunded)
                existing.RefundedAt = DateTime.UtcNow;

            return await _payInvoiceRepo.UpdateAsync(existing);
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            if (payInvoices == null || !payInvoices.Any())
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Payment data is required" };

            var now = DateTime.UtcNow;
            foreach (var invoice in payInvoices)
            {
                invoice.UpdatedAt = now;
            }

            return await _payInvoiceRepo.UpdateRangeAsync(payInvoices);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<bool> { Success = false, Message = "ID is required" };

            var result = await _payInvoiceRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Data = result.Success,
                Message = result.Success ? "Payment deleted successfully" : result.Message
            };
        }

        public async Task<GeneralResponse<bool>> DeleteBySessionIdAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return new GeneralResponse<bool> { Success = false, Message = "Session ID is required" };

            var payment = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            var result = await _payInvoiceRepo.DeleteAsync(payment.Id);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Data = result.Success,
                Message = result.Success ? "Payment deleted successfully" : result.Message
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
                return new GeneralResponse<bool> { Success = false, Message = "IDs are required" };

            var result = await _payInvoiceRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Data = result.Success,
                Message = result.Success ? "Payments deleted successfully" : result.Message
            };
        }

        public async Task<bool> ExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return await _payInvoiceRepo.ExistsAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsBySessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return false;

            var payment = await QueryFirstOrDefaultAsync(p => p.PaymentSessionId == sessionId);
            return payment != null;
        }

        public async Task<int> CountAsync(string invoiceId = null)
        {
            var items = await _payInvoiceRepo.QueryAsync(p => string.IsNullOrEmpty(invoiceId) || p.InvoiceId == invoiceId);
            return items.Count();
        }

        public async Task<int> CountByStatusAsync(PaymentStatus status, string invoiceId = null)
        {
            var items = await _payInvoiceRepo.QueryAsync(p =>
                p.Status == status && (string.IsNullOrEmpty(invoiceId) || p.InvoiceId == invoiceId));
            return items.Count();
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payment.Status != PaymentStatus.Pending)
                return new GeneralResponse<bool> { Success = false, Message = "Only pending payments can be cancelled" };

            payment.Status = PaymentStatus.Cancelled;
            payment.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _payInvoiceRepo.UpdateAsync(payment);
            if (!updateResult.Success)
                return new GeneralResponse<bool> { Success = false, Message = "Failed to cancel payment" };

            return new GeneralResponse<bool> { Success = true, Message = "Payment cancelled successfully", Data = true };
        }

        public async Task<GeneralResponse<bool>> RetryFailedPaymentAsync(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _payInvoiceRepo.GetByIdAsync(paymentId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payment.Status != PaymentStatus.Failed)
                return new GeneralResponse<bool> { Success = false, Message = "Only failed payments can be retried" };

            // Create a new payment session based on the failed payment
            var sessionResponse = await CreatePaymentSessionAsync(payment.InvoiceId, payment.PaymentGatewayType);
            if (!sessionResponse.Success)
                return new GeneralResponse<bool> { Success = false, Message = "Failed to create retry payment session" };

            return new GeneralResponse<bool> { Success = true, Message = "Payment retry initiated successfully", Data = true };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidAmountAsync(string invoiceId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<decimal> { Success = false, Message = "Invoice ID is required" };

            var payments = await _payInvoiceRepo.QueryAsync(p =>
                p.InvoiceId == invoiceId && p.Status == PaymentStatus.Completed);

            var totalAmount = payments.Sum(p => p.Amount);
            return new GeneralResponse<decimal> { Success = true, Data = totalAmount };
        }

        private async Task<PayInvoice> QueryFirstOrDefaultAsync(Expression<Func<PayInvoice, bool>> predicate)
        {
            var items = await _payInvoiceRepo.QueryAsync(predicate);
            return items.FirstOrDefault();
        }

        private string ExtractSessionIdFromUrl(string paymentUrl)
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

        private string GetPaymentMethodIdFromType(PaymentType paymentType) =>
            paymentType.ToString().ToLower();
    }
}