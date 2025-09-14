using invoice.Core.Enums;

namespace invoice.Core.DTO.PaymentResponse
{
    public class PaymentSessionResponse
    {
        public string PaymentUrl { get; set; }
        public string SessionId { get; set; }
        public string RawResponse { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
