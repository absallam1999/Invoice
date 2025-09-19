using invoice.Core.Enums;
using Stripe;

namespace invoice.Core.Entites
{
    public class Invoice : BaseEntity
    {
        public bool Tax { get; set; } = false;
        public string Code { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string? StoreId { get; set; }
        public Store? Store { get; set; }

        public string? ClientId { get; set; }
        public Client? Client { get; set; }

        public string? OrderId { get; set; }
        public Order? Order { get; set; }

        public string? PaymentLinkId { get; set; }
        public PaymentLink? PaymentLink { get; set; }

        public string LanguageId { get; set; }
        public Language Language { get; set; }

        public PayInvoice? PayInvoice { get; set; }
        public List<Payment> Payments { get; set; } = new();
        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}