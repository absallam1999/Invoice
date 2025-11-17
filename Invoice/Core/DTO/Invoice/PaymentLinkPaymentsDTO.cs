using invoice.Core.DTO.PaymentLink;
using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Invoice
{
    public class PaymentLinkPaymentsDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Payments Number must be at least 1")]
        public int PaymentsNumber { get; set; }
        public PaymentType PaymentType { get; set; }

        public GetAllPaymentLinkDTO PaymentLink { get; set; }
    }
}
