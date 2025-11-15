using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.PaymentLink
{
    public class CreatePaymentDTO
    {
        [Required]
        public int PaymentsNumber { get; set; }
        public ClientCreateDTO Client { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
