namespace invoice.Core.DTO.PayInvoice
{
    public class PayInvoiceReadDTO
    {
        public DateTime PaidAt { get; set; }

        public string PaymentMethodId { get; set; }
       
    }
}