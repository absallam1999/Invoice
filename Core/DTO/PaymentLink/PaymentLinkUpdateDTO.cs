using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        public string Purpose { get; set; }
        public decimal? Value { get; set; }
        public string Currency { get; set; }
        public string PaymentsNumber { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public IFormFile Image { get; set; }
        public string Terms { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsageCount { get; set; }
    }
}