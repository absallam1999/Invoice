using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class PaymentMethod
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
<<<<<<< HEAD

        public List<Payment> Payments { get; set; } = new List<Payment>();
=======
        public ICollection<PayInvoice> PayInvoicec { get; set; }
        public ICollection<Payment> Payments { get; set; }
>>>>>>> aaee6c2c23865a8eab5cc4ecec885f7b2c3a347c
    }
}
