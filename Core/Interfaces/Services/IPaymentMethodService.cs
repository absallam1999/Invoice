using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentMethod;
using invoice.Core.Entities;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentMethodService
    {
        Task<GeneralResponse<IEnumerable<PaymentMethodReadDTO>>> GetAllAsync();
        Task<GeneralResponse<PaymentMethod>> GetByIdAsync(string id);
        Task<GeneralResponse<PaymentMethod>> CreateAsync(PaymentType type);
        Task<GeneralResponse<PaymentMethod>> UpdateAsync(string id, PaymentType type);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
    }
}