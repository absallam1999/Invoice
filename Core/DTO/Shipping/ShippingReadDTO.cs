using invoice.Core.Enums;

namespace invoice.Core.DTO.Shipping
{
    public class ShippingReadDTO
    {
        public bool FromStore { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool Delivery { get; set; }

        public string StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
