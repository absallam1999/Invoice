using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.PaymentMethod
{
    public class UpdatePaymentMethodDTO : CreatePaymentMethodDTO
    {
        [Required]
        public string Id { get; set; }
    }
}