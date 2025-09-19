using invoice.Core.DTO;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entites;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPayInvoiceService
    {
        Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(string invoiceId, PaymentType paymentType, string userId = null);
        Task<GeneralResponse<bool>> ProcessPaymentCallbackAsync(string paymentSessionId, PaymentType paymentType, bool isSuccess, string callbackData = null);
        Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType, decimal? amount = null);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusBySessionAsync(string sessionId, PaymentType paymentType);
        Task<GeneralResponse<IEnumerable<PaymentStatusResponse>>> GetPaymentStatusesByInvoiceAsync(string invoiceId);

        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId = null);
        Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId = null);
        Task<GeneralResponse<PayInvoice>> GetBySessionIdAsync(string sessionId, string userId = null);
        Task<GeneralResponse<PayInvoice>> GetByPaymentIdAsync(string paymentId, string userId = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByStatusAsync(PaymentStatus status, string userId = null);

        Task<GeneralResponse<PayInvoice>> CreateAsync(PayInvoice payInvoice);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> CreateRangeAsync(IEnumerable<PayInvoice> payInvoices);

        Task<GeneralResponse<PayInvoice>> UpdateAsync(string id, PayInvoice payInvoice);
        Task<GeneralResponse<PayInvoice>> UpdateStatusAsync(string id, PaymentStatus status, string callbackData = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteBySessionIdAsync(string sessionId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<bool> ExistsBySessionAsync(string sessionId);
        Task<int> CountAsync(string invoiceId = null);
        Task<int> CountByStatusAsync(PaymentStatus status, string invoiceId = null);

        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId);
        Task<GeneralResponse<bool>> RetryFailedPaymentAsync(string paymentId);
        Task<GeneralResponse<decimal>> GetTotalPaidAmountAsync(string invoiceId);
    }
}