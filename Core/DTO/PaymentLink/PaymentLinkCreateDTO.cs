using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkCreateDTO
    {
        [Required]
        public string Link { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        public string PaymentsNumber { get; set; } = null!;
        public string? Description { get; set; }
        public string? Message { get; set; }
        public string? Image { get; set; }
        public string? Terms { get; set; }

        [Required]
        public string InvoiceId { get; set; } = null!;
    }
}