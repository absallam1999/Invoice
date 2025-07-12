using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Payment
{
    public class CreatePaymentDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string InvoiceId { get; set; }

        [Required]
        public string PaymentMethodId { get; set; }
    }
}
