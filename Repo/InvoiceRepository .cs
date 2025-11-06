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

        public async Task<IEnumerable<InvoiceSummaryDto>> GetGroupedByStatusAsync(string userId)
        {
             return await _context.Invoices
                .Where(i => i.UserId == userId && !i.IsDeleted)
                .GroupBy(i => i.InvoiceStatus)
                .Select(g=> new InvoiceSummaryDto
                {
                    InvoiceStatus = g.Key,
                    NumberOfInvoices = g.Count(),
                    TotalCost = g.Sum(x => x.FinalValue)
                })
                .ToListAsync();

        } 
       

        public async Task<IEnumerable<InvoiceSummaryWithDateDto>> GetGroupedByStatusAndDateAsync(string userId)
        {
            return await _context.Invoices
                .Where(i => i.UserId == userId && !i.IsDeleted)
                .GroupBy(i => new { i.InvoiceStatus, Year = i.CreatedAt.Year, Month = i.CreatedAt.Month })
                .Select(g => new InvoiceSummaryWithDateDto
                {
                    InvoiceStatus = g.Key.InvoiceStatus,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NumberOfInvoices = g.Count(),
                    TotalCost = g.Sum(x => x.FinalValue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();
        }





    }
}
