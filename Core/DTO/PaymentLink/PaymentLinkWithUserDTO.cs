using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.User;
using invoice.Core.Entities.utils;

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
        public UserDTO User { get; set; }
        public InvoiceReadDTO Invoice { get; set; }
        public PurchaseCompletionOptions PurchaseOptions { get; set; }
        public PaymentOptionsDTO PaymentOptions { get; set; }
    }
}
