using invoice.Core.DTO.ContactInformation;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.StoreSettings;
using invoice.Models.Entities.utils;

namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkReadDTO
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public int PaymentsNumber { get; set; }
        public int? MaxPaymentsNumber { get; set; }
        public bool IsActivated { get; set; }
        public string Description { get; set; }
        public DateTime? ExpireDate { get; set; } 
        public DateTime CreatedAt { get; set; }

        public PaymentOptionsDTO PaymentOptions { get; set; }
        public PurchaseCompletionOptionsReadDTO purchaseOptions { get; set; }

        public List<GetAllInvoiceDTO>? Invoices { get; set; }

    }
}
