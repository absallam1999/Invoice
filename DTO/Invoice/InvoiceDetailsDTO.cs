using invoice.DTO.InvoiceItem;
using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class InvoiceDetailsDTO
    {
        //url
      
        public string Code { get; set; }
        public DateTime CreateAt { get; set; }
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }
        public string? TermsConditions { get; set; }
        public string InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
        public string Language { get; set; }

        public List<InvoiceItemDetailsDTO> Items { get; set; }

    }
}