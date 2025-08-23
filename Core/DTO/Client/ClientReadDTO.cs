using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Store;
using invoice.Core.Entites;

namespace invoice.Core.DTO.Client
{
    public class ClientReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? TextNumber { get; set; }
        public int InvoiceCount { get; set; }

        public List<StoreReadDTO>? Stores { get; set; }
        public List<InvoiceReadDTO>? Invoices { get; set; }
    }
}
