namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceUpdateDTO
    {
        public string Id { get; set; }
        public string PaymentMethodId { get; set; }
        public string InvoiceId { get; set; }
        public DateTime? PayAt { get; set; }
    }
}