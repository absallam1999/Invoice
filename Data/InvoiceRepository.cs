using invoice.Models;
using invoice.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace invoice.Data
{
    public class InvoiceRepository:Repository<Invoice>, IInvoiceRepository

    {


        public InvoiceRepository(ApplicationDbContext _context):base(_context)
        {
           
        }


        public async Task<string> GenerateInvoiceCode(string userId)
        {
            string NewCode;
            var invoices = await GetAll(userId);

            var codes = invoices
             .Select(item => item.Code)
             .ToList();
            var random = new Random();
            do
            {

                string datePart = DateTime.Now.ToString("yyMMdd");
                string randomPart = random.Next(100, 1000).ToString();
                NewCode = datePart + randomPart;
            }

            while (codes.Contains(NewCode));
            return NewCode;
        }
    }
}
