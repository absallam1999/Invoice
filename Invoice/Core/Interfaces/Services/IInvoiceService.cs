using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO;
using invoice.Core.Enums;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.DTO.Store;

namespace invoice.Core.Interfaces.Services
{
    public interface IInvoiceService

    {
        Task<GeneralResponse<IEnumerable<GetAllInvoiceDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<InvoiceReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<InvoicewithUserDTO>> GetByIdWithUserAsync(string id);
        Task<GeneralResponse<IEnumerable<InvoiceSummaryDto>>> GetInvoicesSummaryAsync(string userId);
        Task<GeneralResponse<IEnumerable<InvoiceSummaryWithDateDto>>> GetInvoicesSummaryWithDateAsync(string userId);

        Task<GeneralResponse<InvoiceReadDTO>> GetByCodeAsync(string code, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> SearchAsync(string keyword, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> CreateAsync(InvoiceCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> CreateRangeAsync(IEnumerable<InvoiceCreateDTO> dtos, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> UpdateAsync(string id, InvoiceUpdateDTO dto, string userId);
        Task<GeneralResponse<bool>> PayAsync(string id, string userId, PayInvoiceCreateDTO dto = null);
        Task<GeneralResponse<bool>> RefundAsync(string id, string userId);
        Task<GeneralResponse<bool>> ChangeOrderStatus(string id, ChangeOrderStatusDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> UpdateRangeAsync(IEnumerable<InvoiceUpdateDTO> dtos, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId, InvoiceType? invoicetype = null);

        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByClientAsync(string clientId, string userId);
        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStatusAsync(InvoiceStatus status, string userId);
        Task<GeneralResponse<POSInvoicesResultDTO>> GetForPOSAsync(InvoiceType type, string userId);
        Task<GeneralResponse<IEnumerable<GetAllInvoiceDTO>>> GetAllForStoreAsync(string userId);
        Task<GeneralResponse<IEnumerable<GetAllInvoiceDTO>>> GetByTypeAsync(string userId,InvoiceType invoicetype);

        Task<GeneralResponse<decimal>> GetTotalValueAsync(string userId);
        Task<GeneralResponse<decimal>> GetTotalFinalValueAsync(string userId);

        Task<GeneralResponse<bool>> AddPaymentAsync(string invoiceId, PaymentCreateDTO paymentDto, string userId);

    }
}