using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.Entities;
using System.Linq.Expressions;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentLinkService
    {
        Task<GeneralResponse<PaymentLinkReadDTO>> CreateAsync(PaymentLinkCreateDTO dto, string userId);
        Task<GeneralResponse<PaymentLinkReadDTO>> UpdateAsync(string id, PaymentLinkUpdateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentLinkUpdateDTO> dtos, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);
        Task<GeneralResponse<bool>> ActivatePaymentLinkAsync(string id, string userId);
        Task<GeneralResponse<PaymentLinkWithUserDTO>> GetBySlug(string slug);
        Task<GeneralResponse<PaymentLinkReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<GetAllPaymentLinkDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> QueryAsync(Expression<Func<PaymentLink, bool>> predicate, string userId);
        
        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
        Task<GeneralResponse<object>> CreatePaymentAsync(CreatePaymentDTO dto, string id, string userId);

    }
}
