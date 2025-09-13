using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class CustomPaymentLinkCreateDTO
    {
        [Required]
        public string Purpose { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        public string Currency { get; set; } = "USD";
        public string Description { get; set; }
        public string Message { get; set; }
        public IFormFile Image { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }
    }
}