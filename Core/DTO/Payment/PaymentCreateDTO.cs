namespace invoice.Core.DTO.Payment
{
    public class PaymentCreateDTO
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string? InvoiceId { get; set; }
        public string? PaymentMethodId { get; set; }
        public string? PaymentLinkId { get; set; }
    }
}
