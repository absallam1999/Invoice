using invoice.DTO.InvoiceItem;
using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class InvoiceDetailsDTO
    {
        //url
      
        public string Code { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? PayAt { get; set; }
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }
        public string? TermsConditions { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string ClintId { get; set; }
        public string ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
        public string? PaymentMethod { get; set; }

        public string Language { get; set; }
        public List<InvoiceItemDetailsDTO> Items { get; set; }

    }
}