using invoice.Core.Enums;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Language;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceReadDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }

        public string UserId { get; set; }
        public string? StoreId { get; set; }
        public StoreReadDTO? Store { get; set; }

        public string? ClientId { get; set; }
        public ClientSummaryDTO? Client { get; set; }

        public string LanguageId { get; set; }
        public LanguageReadDTO Language { get; set; }

        public IEnumerable<PaymentReadDTO> Payments { get; set; } = new List<PaymentReadDTO>();
        public IEnumerable<InvoiceItemReadDTO> InvoiceItems { get; set; } = new List<InvoiceItemReadDTO>();
        public IEnumerable<PaymentLinkReadDTO> PaymentLinks { get; set; } = new List<PaymentLinkReadDTO>();
    }
}
