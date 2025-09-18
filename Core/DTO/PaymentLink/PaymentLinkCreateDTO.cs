using invoice.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
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

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a valid 3-letter ISO code")]
        public string Currency { get; set; } = "USD";

        [Range(1, int.MaxValue, ErrorMessage = "Payments number must be at least 1")]
        public int PaymentsNumber { get; set; } = 1;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public IFormFile Image { get; set; }

        [StringLength(2000, ErrorMessage = "Terms cannot exceed 2000 characters")]
        public string Terms { get; set; }

        public bool GenerateGatewayLink { get; set; } = true;

        public DateTime? ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(3);

        [Range(1, int.MaxValue, ErrorMessage = "Max usage must be at least 1")]
        public int? MaxUsageCount { get; set; }

        public string? InvoiceId { get; set; }

        public string? StoreId { get; set; }

        public string? ClientId { get; set; }

        [Required(ErrorMessage = "LanguageId is required")]
        public string LanguageId { get; set; }

        public bool? Tax { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }
}
