using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Payment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentLink PaymentLink { get; set; }
    }
}
