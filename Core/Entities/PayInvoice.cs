using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class PayInvoice : BaseEntity
    {
        public DateTime PaidAt { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentUrl { get; set; }
        public string PaymentSessionId { get; set; }
        public string PaymentGatewayResponse { get; set; }

        public PaymentType PaymentGatewayType { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public decimal? RefundAmount { get; set; }
        public DateTime? RefundedAt { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
