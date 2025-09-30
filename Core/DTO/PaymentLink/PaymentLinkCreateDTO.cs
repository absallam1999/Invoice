using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkCreateDTO
    {
        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(200, ErrorMessage = "Purpose cannot exceed 200 characters")]
        public string Purpose { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Payments number must be at least 1")]
        public int PaymentsNumber { get; set; } = 1;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public IFormFile Image { get; set; }

        [StringLength(2000, ErrorMessage = "Terms cannot exceed 2000 characters")]
        public string Terms { get; set; }
        public string LanguageId { get; set; }

        public bool? Tax { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }
}
