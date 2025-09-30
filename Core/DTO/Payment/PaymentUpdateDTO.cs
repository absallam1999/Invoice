using invoice.Core.Enums;

namespace invoice.Core.DTO.Payment
{
    public class PaymentUpdateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; }

        public string InvoiceId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
