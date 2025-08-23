using invoice.Core.DTO;
using invoice.Core.DTO.Payment;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetAllAsync(string userId = null);
        Task<GeneralResponse<PaymentReadDTO>> GetByIdAsync(string id, string userId = null);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByInvoiceIdAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null);

        Task<GeneralResponse<PaymentReadDTO>> CreateAsync(PaymentCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> CreateRangeAsync(IEnumerable<PaymentCreateDTO> dtos, string userId);

        Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentUpdateDTO> dtos, string userId);

        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId = null);
        Task<int> CountAsync(string userId = null);

        Task<GeneralResponse<string>> ProcessPaymentAsync(string paymentMethodId, PaymentCreateDTO dto, string userId);

        Task<GeneralResponse<decimal>> GetTotalPaidByInvoiceAsync(string invoiceId, string userId = null);
    }
}
