using invoice.Core.DTO.InvoiceItem;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceUpdateDTO
    {
        public bool Tax { get; set; } = false;
        public decimal Value { get; set; }
        public string Currency { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public string? TermsConditions { get; set; }

        public string? ClientId { get; set; }
        public string LanguageId { get; set; }

        public IEnumerable<InvoiceItemUpdateDTO>? InvoiceItems { get; set; }
    }
}
