using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Payment;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceCreateDTO
    {
        public string Code { get; set; }
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }

        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }

        public string? StoreId { get; set; }
        public string? ClientId { get; set; }
        public string LanguageId { get; set; }

        public IEnumerable<PaymentCreateDTO>? Payments { get; set; }
        public IEnumerable<InvoiceItemCreateDTO>? InvoiceItems { get; set; }
    }
}
