using System.ComponentModel.DataAnnotations;
using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class CreateInvoiceDTO
    {
        [Required]
        public string Number { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        public string TaxNumber { get; set; }

        public double Value { get; set; }

        public string Description { get; set; }

        [Required]
        public InvoiceStatus InvoiceStatus { get; set; }

        [Required]
        public InvoiceType InvoiceType { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string StoreId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string LanguageId { get; set; }
    }
}
