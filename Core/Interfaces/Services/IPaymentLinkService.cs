using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
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
        Task<GeneralResponse<IEnumerable<bool>>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<GeneralResponse<PaymentLinkReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> QueryAsync(Expression<Func<PaymentLink, bool>> predicate, string userId);

        Task<GeneralResponse<PaymentLinkReadDTO>> GenerateLinkAsync(string invoiceId, decimal value, string userId);
        Task<GeneralResponse<InvoiceReadDTO>> RecalculateInvoiceTotalsAsync(string invoiceId, string userId);
        Task<GeneralResponse<bool>> AttachPaymentAsync(string paymentLinkId, Payment payment, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
    }
}
