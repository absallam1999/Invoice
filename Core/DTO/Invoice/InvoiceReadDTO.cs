using invoice.Core.Enums;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Language;
using invoice.Core.DTO.Tax;
using invoice.Core.DTO.PayInvoice;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceReadDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public bool Tax { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PayAt { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string? TermsConditions { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public string? ClientId { get; set; }
        public ClientSummaryDTO? Client { get; set; }
        public LanguageReadDTO Language { get; set; }
     

        public TaxReadDTO TaxInfo { get; set; }
        public PayInvoiceReadDTO PayInvoice { get; set; }

        public IEnumerable<InvoiceItemReadDTO> InvoiceItems { get; set; } = new List<InvoiceItemReadDTO>();
        public OrderDTO Order { get; set; } = new OrderDTO();
        public PaymentLinkPaymentsDTO PaymentLinkPayment { get; set; }
    }
}