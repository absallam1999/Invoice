using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkUpdateDTO
    {
        [Required]
        public string Id { get; set; } = null!;

        public string? Link { get; set; }
        public decimal? Value { get; set; }
        public string? PaymentsNumber { get; set; }
        public string? Description { get; set; }
        public string? Message { get; set; }
        public string? Image { get; set; }
        public string? Terms { get; set; }
    }
}
