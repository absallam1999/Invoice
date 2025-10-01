using invoice.Core.Entities;
using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class Invoice : BaseEntity
    {
        public string Code { get; set; }
        public decimal Value { get; set; }
        public bool Tax { get; set; } = false;
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }

        public string? TermsConditions { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 
        public Order? Order { get; set; }
        public PaymentLinkPayments? PaymentLinkPayment { get; set; }
        public string? ClientId { get; set; }
        public Client? Client { get; set; }

        public string LanguageId { get; set; } = "ar";
        public Language Language { get; set; }

        public List<Payment> Payments { get; set; } = new();
        public PayInvoice? PayInvoice { get; set; }
        public List<InvoiceItem>? InvoiceItems { get; set; } 

    }
}