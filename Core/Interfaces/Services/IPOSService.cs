using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface IPOSService
    {
        Task<GeneralResponse<Product>> GetProductByNameAsync(string name, string userId);
        Task<GeneralResponse<IEnumerable<Product>>> SearchProductsAsync(string keyword, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> CreateDraftInvoiceAsync(string storeId, string userId);
        Task<GeneralResponse<InvoiceReadDTO>> AddItemToInvoiceAsync(string invoiceId, string productId, int quantity, string userId);
        Task<GeneralResponse<InvoiceReadDTO>> UpdateInvoiceItemAsync(string invoiceItemId, int newQuantity, string userId);
        Task<GeneralResponse<bool>> RemoveInvoiceItemAsync(string invoiceItemId, string userId);

        Task<GeneralResponse<InvoiceReadDTO>> FinalizeSaleAsync(string invoiceId, string paymentMethodId, decimal paidAmount, string userId);
        Task<GeneralResponse<bool>> CancelSaleAsync(string invoiceId, string userId);

        Task<GeneralResponse<bool>> RefundInvoiceAsync(string invoiceId, string userId);

        Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetPOSInvoicesAsync(string storeId, DateTime? date, string userId);
        Task<GeneralResponse<decimal>> GetDailySalesTotalAsync(string storeId, DateTime date, string userId);
    }
}
