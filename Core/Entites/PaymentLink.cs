namespace invoice.Core.Entites
{
    public class PaymentLink : BaseEntity
    {
        public string Link { get; set; }
        public string GatewaySessionId { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; } = "USD";
        public string Purpose { get; set; }
        public string PaymentsNumber { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string Terms { get; set; }

        public string CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public List<Payment> Payments { get; set; } = new();
    }
}