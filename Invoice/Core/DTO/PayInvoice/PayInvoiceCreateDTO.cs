namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceCreateDTO
    {
        public string PaymentMethodId { get; set; } = "ca";
        public DateTime? PayAt { get; set; } = DateTime.UtcNow;
    }
}