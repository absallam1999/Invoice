using invoice.Core.DTO.Payment;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkReadDTO
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public string GatewaySessionId { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string Purpose { get; set; }
        public string PaymentsNumber { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string Terms { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string InvoiceId { get; set; }

        public IEnumerable<PaymentReadDTO> Payments { get; set; } = new List<PaymentReadDTO>();
    }
}
