using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class OrderDTO
    {

        public bool IsPaid { get; set; } = false;

        public ShippingMethod ShippingMethod { get; set; }
        public PaymentType PaymentType { get; set; }

        public OrderStatus? OrderStatus { get; set; } = null;
        public decimal DeliveryCost { get; set; } = 0;

    }
}