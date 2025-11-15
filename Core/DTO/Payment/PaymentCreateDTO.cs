using invoice.Helpers;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Payment
{
    public class PaymentCreateDTO
    {
        [Required(ErrorMessage = "Payment name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a valid 3-letter ISO code")]
        public string Currency { get; set; } = "USD";

        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
        public decimal Cost { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max usage must be at least 1")]
        public int? MaxUsageCount { get; set; }

        public DateTime? ExpiresAt { get; private set; } = GetSaudiTime.Now().AddDays(3);

        [Required(ErrorMessage = "Invoice ID is required")]
        public string InvoiceId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public string ClientId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ClientEmail { get; set; }

        public string PaymentMethodId { get; set; }

        public Dictionary<string, string>? Metadata { get; set; } = new Dictionary<string, string>();
    }
}