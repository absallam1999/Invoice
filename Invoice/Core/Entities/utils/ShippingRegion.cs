
using invoice.Core.Entities;

namespace invoice.Models.Entities.utils
{
    public class ShippingRegion:BaseEntity
    {

        public string Region { get; set; }     
        public decimal DeliveryFee { get; set; }

       // public string ShippingId { get; set; }
      //  public Shipping Shipping { get; set; }

    }
}
