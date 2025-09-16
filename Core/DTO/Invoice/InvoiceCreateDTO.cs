using invoice.Core.DTO.InvoiceItem;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceCreateDTO
    {
        public bool Tax { get; set; } = false;
        public string? Currency { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }

        public string? StoreId { get; set; }
        public string? ClientId { get; set; }
        public string? LanguageId { get; set; }

        public IEnumerable<InvoiceItemCreateDTO>? InvoiceItems { get; set; }
    }
}
