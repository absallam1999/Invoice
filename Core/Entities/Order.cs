using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class Order:BaseEntity
    {
        public bool IsPaid { get; set; }=false;
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentType PaymentType { get; set; } 

        public OrderStatus? OrderStatus { get; set; } = null;
        public decimal DeliveryCost { get; set; } =0;

    }
}
