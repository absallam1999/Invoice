using invoice.Core.DTO.PaymentMethod;

namespace invoice.Core.DTO.Payment
{
    public class PaymentReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public string InvoiceId { get; set; }
        public string? PaymentLinkId { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethodReadDTO PaymentMethod { get; set; }
    }
}
