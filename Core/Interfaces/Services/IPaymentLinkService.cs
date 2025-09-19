using invoice.Core.DTO;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;
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

        Task<GeneralResponse<PaymentLinkReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> QueryAsync(Expression<Func<PaymentLink, bool>> predicate, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
        Task<int> CountActiveAsync(string userId);
        Task<int> CountByInvoiceAsync(string invoiceId, string userId);
    }
}
