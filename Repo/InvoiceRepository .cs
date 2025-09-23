using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Repo;
using System.Text.RegularExpressions;

namespace invoice.Repo
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Invoice> _dbSet;
         
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Invoice>();
        }

        public async Task<IEnumerable<IGrouping<InvoiceStatus, Invoice>>> GetGroupedByStatusAsync(string userId)
        {
             return await _context.Invoices
                .Where(i => i.UserId == userId && !i.IsDeleted)
                .GroupBy(i => i.InvoiceStatus)
                .ToListAsync();

        }



       
    }
}
