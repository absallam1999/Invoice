using System.ComponentModel.DataAnnotations;
using invoice.DTO.InvoiceItem;
using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class InvoiceInfoDTO
    {
        public DateTime? CreateAt { get; set; }

        //public string TaxNumber { get; set; }
        //public string ClientId { get; set; }
        public string? ClientId { get; set; }
        public string? LanguageId { get; set; }
        public string? TermsConditions { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public List<InvoiceItemDTO> Items { get; set; }

    }
}
