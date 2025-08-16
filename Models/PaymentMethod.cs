using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class PaymentMethod
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PayInvoice> PayInvoices { get; set; }
    }
}
