using invoice.Core.DTO;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface IPayInvoiceService
    {
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId = null);
        Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId = null);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null);

        Task<GeneralResponse<PayInvoice>> CreateAsync(PayInvoice payInvoice);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> CreateRangeAsync(IEnumerable<PayInvoice> payInvoices);

        Task<GeneralResponse<PayInvoice>> UpdateAsync(string id, PayInvoice payInvoice);
        Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync(string invoiceId = null);
    }
}