using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<InvoiceReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<InvoiceReadDTO>> GetByCodeAsync(string code, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> SearchAsync(string keyword, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> CreateAsync(InvoiceCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> CreateRangeAsync(IEnumerable<InvoiceCreateDTO> dtos, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> UpdateAsync(string id, InvoiceUpdateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> UpdateRangeAsync(IEnumerable<InvoiceUpdateDTO> dtos, string userId);

        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);

        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByClientAsync(string clientId, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStoreAsync(string storeId, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStatusAsync(InvoiceStatus status, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByTypeAsync(InvoiceType type, string userId);

        Task<GeneralResponse<decimal>> GetTotalValueAsync(string userId);
        Task<GeneralResponse<decimal>> GetTotalFinalValueAsync(string userId);

        Task<GeneralResponse<bool>> AddPaymentAsync(string invoiceId, PaymentCreateDTO paymentDto, string userId);
        Task<GeneralResponse<bool>> GeneratePaymentLinkAsync(PaymentLinkCreateDTO dto, string userId);

        Task<GeneralResponse<bool>> MarkAsPaidAsync(string invoiceId, string userId);
        Task<GeneralResponse<bool>> CancelAsync(string invoiceId, string userId);
    }
}
