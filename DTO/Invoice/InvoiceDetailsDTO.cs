using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class InvoiceDetailsDTO
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public DateTime CreateAt { get; set; }
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public string InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public string ClientId { get; set; }
        public string LanguageId { get; set; }
    }
}