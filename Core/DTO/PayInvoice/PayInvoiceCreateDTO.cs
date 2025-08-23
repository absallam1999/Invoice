namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceCreateDTO
    {
        public string PaymentMethodId { get; set; }
        public string InvoiceId { get; set; }
        public DateTime? PayAt { get; set; } = DateTime.UtcNow;
    }
}