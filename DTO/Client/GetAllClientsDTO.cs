
using invoice.DTO.Invoice;

namespace invoice.DTO.Client
{
    public class GetAllClientsDTO
    {

        public string ClientId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int InvoiceCount { get; set; }

       


    }
}
