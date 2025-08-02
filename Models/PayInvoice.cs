namespace invoice.Models
{
    public class PayInvoice
    {


        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? PayAt { get; set; }= DateTime.UtcNow;


        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
   
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
