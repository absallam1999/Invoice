using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetAllAsync(string userId = null);
        Task<GeneralResponse<PaymentReadDTO>> GetByIdAsync(string id, string userId = null);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByInvoiceIdAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null);

        Task<GeneralResponse<PaymentReadDTO>> CreateAsync(PaymentCreateDTO dto, string userId);
        Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);

        Task<bool> ExistsAsync(string id, string userId = null);
        Task<int> CountAsync(string userId = null);
        Task<int> CountByInvoiceAsync(string invoiceId, string userId = null);
        Task<int> CountByPaymentMethodAsync(string paymentMethodId, string userId = null);

        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, string userId);
        Task<GeneralResponse<bool>> MarkAsCompletedAsync(string paymentId, string userId);
        Task<GeneralResponse<bool>> MarkAsFailedAsync(string paymentId, string failureReason, string userId);

        Task<GeneralResponse<decimal>> GetTotalPaidByInvoiceAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<decimal>> GetTotalPaidByPaymentMethodAsync(string paymentMethodId, string userId = null);
        Task<GeneralResponse<decimal>> GetTotalPaidByUserAsync(string userId);

        Task<GeneralResponse<IDictionary<string, decimal>>> GetMonthlyRevenueAsync(int year, string userId = null);
        Task<GeneralResponse<IDictionary<string, decimal>>> GetRevenueByPaymentMethodAsync(string userId = null);

        Task<GeneralResponse<bool>> ExpirePaymentAsync(string paymentId, string userId);
        Task<GeneralResponse<bool>> ReactivatePaymentAsync(string paymentId, string userId);
    }
}
