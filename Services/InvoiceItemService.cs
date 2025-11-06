using invoice.Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Repo;

namespace invoice.Services
{
    public class InvoiceItemsService : IInvoiceItemsService
    {
        private readonly IRepository<InvoiceItem> _invoiceItemRepo;

        public InvoiceItemsService(IRepository<InvoiceItem> invoiceItemRepo)
        {
            _invoiceItemRepo = invoiceItemRepo;
        }

        public async Task<GeneralResponse<InvoiceItem>> CreateAsync(InvoiceItem item)
        {
            var response = await _invoiceItemRepo.AddAsync(item);
            return response.Success
                ? new GeneralResponse<InvoiceItem> { Success = true, Data = response.Data }
                : new GeneralResponse<InvoiceItem> { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<InvoiceItem>> UpdateAsync(InvoiceItem item)
        {
            var response = await _invoiceItemRepo.UpdateAsync(item);
            return response.Success
                ? new GeneralResponse<InvoiceItem> { Success = true, Data = response.Data }
                : new GeneralResponse<InvoiceItem> { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<InvoiceItem>> DeleteAsync(string id)
        {
            var response = await _invoiceItemRepo.DeleteAsync(id);
            return response.Success
                ? new GeneralResponse<InvoiceItem> { Success = true, Data = response.Data }
                : new GeneralResponse<InvoiceItem> { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceItem>>> DeleteByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            var items = (await _invoiceItemRepo.QueryAsync(i => i.InvoiceId == invoiceId)).ToList();
            if (!items.Any())
                return new GeneralResponse<IEnumerable<InvoiceItem>> { Success = false, Message = "No items found for this invoice" };

            var response = await _invoiceItemRepo.DeleteRangeAsync(items.Select(i => i.Id));
            return response.Success
                ? new GeneralResponse<IEnumerable<InvoiceItem>> { Success = true, Data = response.Data }
                : new GeneralResponse<IEnumerable<InvoiceItem>> { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceItem>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            var items = await _invoiceItemRepo.QueryAsync(i => i.InvoiceId == invoiceId);
            return new GeneralResponse<IEnumerable<InvoiceItem>> { Success = true, Data = items };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceItem>>> GetByProductIdAsync(string productId, string userId = null)
        {
            var items = await _invoiceItemRepo.QueryAsync(i => i.ProductId == productId);
            return new GeneralResponse<IEnumerable<InvoiceItem>> { Success = true, Data = items };
        }

        public async Task<GeneralResponse<decimal>> GetInvoiceTotalAsync(string invoiceId, string userId = null)
        {
            var items = await _invoiceItemRepo.QueryAsync(i => i.InvoiceId == invoiceId);
            var total = items.Sum(i => i.UnitPrice * i.Quantity);
            return new GeneralResponse<decimal> { Success = true, Data = total };
        }

        public async Task<bool> ExistsAsync(string id, string userId = null)
        {
            return await _invoiceItemRepo.ExistsAsync(i => i.Id == id);
        }

        public async Task<int> CountAsync(string userId = null)
        {
            return await _invoiceItemRepo.CountAsync();
        }
    }
}