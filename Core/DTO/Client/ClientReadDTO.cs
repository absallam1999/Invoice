using invoice.Core.DTO.Invoice;


namespace invoice.Core.DTO.Client
{
    public class ClientReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }

        public int InvoiceCount { get; set; }
        public decimal InvoiceTotal { get; set; }

        public List<GetAllInvoiceDTO>? Invoices { get; set; }
    }
}