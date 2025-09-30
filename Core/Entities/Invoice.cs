using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class Invoice : BaseEntity
    {
        public string Code { get; set; }
        public decimal Value { get; set; }
        public decimal FinalValue { get; set; }

        public bool Tax { get; set; } = false;
        public decimal? DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }

        public InvoiceType InvoiceType { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }

        public string? TermsConditions { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string? PaymentLinkId { get; set; }
        public PaymentLink? PaymentLink { get; set; }
        
        public string? ClientId { get; set; }
        public Client? Client { get; set; }

        public string LanguageId { get; set; } = "ar";
        public Language Language { get; set; }

        public Order? Order { get; set; }
        public Commission? Commission { get; set; }
        public PayInvoice? PayInvoice { get; set; }
        public List<Payment> Payments { get; set; } = new();
        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}