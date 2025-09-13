using invoice.Core.Enums;

namespace invoice.Core.DTO.PaymentResponse
{
    public class PaymentStatusResponse
    {
        public string PaymentId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public DateTime LastUpdated { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
