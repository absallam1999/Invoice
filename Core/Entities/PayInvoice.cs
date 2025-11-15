namespace invoice.Core.Entities
{
    public class PayInvoice : BaseEntity
    {
        public DateTime PaidAt { get; set; }

        public string PaymentMethodId { get; set; } = "ca";
        public PaymentMethod PaymentMethod { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}