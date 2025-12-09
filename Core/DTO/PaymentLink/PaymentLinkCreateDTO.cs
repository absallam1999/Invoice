using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkCreateDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Slug must contain only letters, " +
          "numbers, dashes or underscores without spaces.")]
        public string Slug { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        public int? MaxPaymentsNumber { get; set; } = null;
        public DateTime? ExpireDate { get; set; } = null;
        public string Currency { get; set; }

      
       
    }
}