using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class GetAllInvoiceDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public OrderStatus? OrderStatus { get; set; }

        public InvoiceType InvoiceType { get; set; }

        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
      


    }
}