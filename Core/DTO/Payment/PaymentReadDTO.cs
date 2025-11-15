using invoice.Core.DTO.PaymentMethod;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Payment
{
    public class PaymentReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public decimal Cost { get; set; }
        public string Currency { get; set; }
        public string GatewaySessionId { get; set; }

        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string? UserId { get; set; }

        public string InvoiceId { get; set; }

        public string? PaymentMethodId { get; set; }
        public PaymentMethodReadDTO? PaymentMethod { get; set; }
    }
}
