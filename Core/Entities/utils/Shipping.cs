using invoice.Core.Entities;

namespace invoice.Models.Entities.utils
{
    public class Shipping
    {
        public bool FromStore { get; set; } = true;
        public bool Delivery { get; set; } = false;


        public string? Region { get; set; }
        public decimal? DeliveryFee { get; set; }

    }
}
