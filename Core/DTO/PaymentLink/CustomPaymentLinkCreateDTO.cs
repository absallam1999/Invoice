using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class CustomPaymentLinkCreateDTO
    {
        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(200, ErrorMessage = "Purpose cannot exceed 200 characters")]
        public string Purpose { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a valid 3-letter ISO code")]
        public string Currency { get; set; } = "USD";

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public IFormFile Image { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max usage must be at least 1")]
        public int? MaxUsageCount { get; set; }

        [Required(ErrorMessage = "Payment type is required")]
        public PaymentType PaymentType { get; set; }
        public string ClientId { get; set; }

    }
}
