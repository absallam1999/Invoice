using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class PaymentLinkPayments:BaseEntity
    {
        public int PaymentsNumber { get; set; }
        public PaymentType PaymentType { get; set; }

        public string PaymentLinkId { get; set; }
        public PaymentLink PaymentLink { get; set; }     
        
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
