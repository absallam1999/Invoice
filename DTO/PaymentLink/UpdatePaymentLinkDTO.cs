using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.PaymentLink
{
    public class UpdatePaymentLinkDTO : CreatePaymentLinkDTO
    {
        [Required]
        public string Id { get; set; }
    }
}
