using invoice.Core.Enums;

namespace invoice.Core.Entites
{
    public class PayInvoice : BaseEntity
    {
        public DateTime PaidAt { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string PaymentMethodId { get; set; } = "cash";
        public PaymentMethod PaymentMethod { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public PaymentType PaymentGatewayType { get; set; }
        public string PaymentSessionId { get; set; }
        public string PaymentGatewayResponse { get; set; }

        public decimal? RefundAmount { get; set; }
        public DateTime? RefundedAt { get; set; }
    }
}
