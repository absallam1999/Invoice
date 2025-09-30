using invoice.Core.Enums;

namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceCreateDTO
    {
        public string PaymentMethodId { get; set; }

        public DateTime? PayAt { get; set; } = DateTime.UtcNow;
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }

        public PaymentType? PaymentGatewayType { get; set; }
        public string? PaymentUrl { get; set; }
        public string? PaymentSessionId { get; set; }
        public string? PaymentGatewayResponse { get; set; }
    }
}
