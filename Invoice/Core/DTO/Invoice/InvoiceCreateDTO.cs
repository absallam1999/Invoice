using invoice.Core.DTO.InvoiceItem;
using invoice.Core.Enums;
using System.Text.Json.Serialization;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceCreateDTO
    {
        [JsonPropertyName("Tax")]

        public bool HaveTax { get; set; } = false;
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }

        public string? ClientId { get; set; }
        public string? LanguageId { get; set; }

        public IEnumerable<InvoiceItemCreateDTO>? InvoiceItems { get; set; }

    }
}