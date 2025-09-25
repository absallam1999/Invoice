using invoice.Core.DTO.ContactInformation;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.StoreSettings;
using invoice.Models.Entities.utils;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkReadDTO
    {
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string PaymentsNumber { get; set; }
        public int? MaxPaymentsNumber { get; set; }
        public bool IsActivated { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }


        
        public List<GetAllInvoiceDTO>? Invoices { get; set; }
        public PaymentOptionsDTO PaymentOptions { get; set; }
        public PurchaseCompletionOptions PurchaseOptions { get; set; }
    }
}
