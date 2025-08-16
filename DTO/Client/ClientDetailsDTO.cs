using invoice.DTO.Invoice;

namespace invoice.DTO.Client
{
    public class ClientDetailsDTO
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? TextNumber { get; set; }

        public DateTime CreateAt { get; set; }
       
        public int InvoiceCount { get; set; }

        public List<GetAllInvoiceDTO> Invoices { get; set; }
    }
}