using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class PaymentMethod
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public ICollection<PayInvoice> PayInvoicec { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
