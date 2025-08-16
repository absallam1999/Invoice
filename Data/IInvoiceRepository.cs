using invoice.Models;

namespace invoice.Data
{
    public interface IInvoiceRepository :IRepository<Invoice>
    {
        public  Task<string> GenerateInvoiceCode(string userId);
    }
}
