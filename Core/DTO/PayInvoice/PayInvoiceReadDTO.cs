namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceReadDTO
    {
        public string Id { get; set; }
        public DateTime PayAt { get; set; }
        public string? PaymentUrl{ get; set; }
        public string PaymentSessionId { get; set; }
        public string PaymentMethodId { get; set; }
        public string InvoiceId { get; set; }
    }
}
