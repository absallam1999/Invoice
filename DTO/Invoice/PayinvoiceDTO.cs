namespace invoice.DTO.Invoice
{
    public class PayinvoiceDTO
    {

        
        public DateTime? PayAt { get; set; }= DateTime.UtcNow;
        public string PaymentMethodId { get; set; } //default method
        //public string InvoiceId { get; set; }
     
    }
}
