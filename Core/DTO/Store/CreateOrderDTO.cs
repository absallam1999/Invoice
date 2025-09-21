using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Store
{
    public class CreateOrderDTO
    {

        public ClientCreateDTO Client { get; set; }
        public InvoiceCreateDTO Invoice { get; set; }
        public decimal DeliveryCost { get; set; } = 0;
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentType PaymentType { get; set; }

    }
}
