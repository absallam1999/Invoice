namespace invoice.Core.Entities
{
    public class Payment : BaseEntity
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string? PaymentLinkId { get; set; }
        public PaymentLink PaymentLink { get; set; }
    }
}
