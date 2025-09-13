using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkCreateDTO
    {
        [Required]
        public string Purpose { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        public string Currency { get; set; } = "USD";
        public string PaymentsNumber { get; set; } = "1";
        public string Description { get; set; }
        public string Message { get; set; }
        public IFormFile Image { get; set; }
        public string Terms { get; set; }
        public bool GenerateGatewayLink { get; set; } = true;
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }

        public string InvoiceId { get; set; }
    }
}