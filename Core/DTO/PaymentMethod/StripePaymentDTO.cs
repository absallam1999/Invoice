using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentMethod
{
    public class StripePaymentDTO
    {
        [Required] 
        public string InvoiceId { get; set; }
        
        [Required] 
        public string Code { get; set; }
        
        [Required] 
        public decimal Cost { get; set; }
        
        [EmailAddress]
        public string CustomerEmail { get; set; }
        
        [Required]
        public string PaymentMethodId { get; set; }
    }
}
