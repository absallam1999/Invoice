using invoice.Core.DTO;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface IInvoiceItemsService
    {
        Task<GeneralResponse<InvoiceItem>> CreateAsync(InvoiceItem item);
        Task<GeneralResponse<InvoiceItem>> UpdateAsync(InvoiceItem item);
        Task<GeneralResponse<InvoiceItem>> DeleteAsync(string id);
        Task<GeneralResponse<IEnumerable<InvoiceItem>>> DeleteByInvoiceIdAsync(string invoiceId, string userId = null);

        Task<GeneralResponse<IEnumerable<InvoiceItem>>> GetByInvoiceIdAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<IEnumerable<InvoiceItem>>> GetByProductIdAsync(string productId, string userId = null);
        Task<GeneralResponse<decimal>> GetInvoiceTotalAsync(string invoiceId, string userId = null);

        Task<bool> ExistsAsync(string id, string userId = null);
        Task<int> CountAsync(string userId = null);
    }
}