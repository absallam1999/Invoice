using invoice.Core.Entities;

namespace invoice.Models.Entities.utils
{
    public class Shipping/*: BaseEntity*/
    {
        public bool FromStore { get; set; } = true;
        public bool Delivery { get; set; } = false;


        public string? Region { get; set; }
        public decimal? DeliveryFee { get; set; }
        //public string StoreId { get; set; }
        //public Store Store { get; set; }
        // public ICollection<ShippingRegion>? Regions { get; set; } 



    }
}
