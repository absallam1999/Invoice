using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public PaymentType Name { get; set; }

        public List<Payment> Payments { get; set; } = new();
        public List<PayInvoice> PayInvoices { get; set; } = new();
    }
}