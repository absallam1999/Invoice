using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.PurchaseCompletionOptions;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Payment
{
    public class PaymentUpdateDTO
    {

        [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Slug must contain only letters, " +
         "numbers, dashes or underscores without spaces.")]
        public string Slug { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public int? MaxPaymentsNumber { get; set; } = null;
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsActivated { get; set; }

        public PaymentOptionsDTO PaymentOptions { get; set; }
        public PurchaseCompletionOptionsUpdateDTO PurchaseOptions { get; set; }
       
    }
}
