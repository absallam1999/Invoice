using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Shipping
{
    public class ShippingUpdateDTO
    {
        public bool? FromStore { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool? Delivery { get; set; }


        [Required(ErrorMessage = "StoreId is required")]
        public string StoreId { get; set; }
    }
}
