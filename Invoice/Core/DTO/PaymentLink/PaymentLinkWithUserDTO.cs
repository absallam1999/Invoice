using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Core.DTO.Tax;
using invoice.Core.DTO.User;
using invoice.Models.Entities.utils;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkWithUserDTO
    {
        public string Id { get; set; }
        public decimal Value { get; set; }
        public bool IsActivated { get; set; } 
        public DateTime? ExpireDate { get; set; } 
        public int? RemainingPaymentsNumber { get; set; }  
        public string? Description { get; set; }
        public string Currency { get; set; }


        public UserDTO User { get; set; }
        public PurchaseCompletionOptionsReadDTO PurchaseOptions { get; set; }
        public PaymentOptionsDTO PaymentOptions { get; set; }
        public TaxReadDTO? Tax { get; set; }

    }
}
