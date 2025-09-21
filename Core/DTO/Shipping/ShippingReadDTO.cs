﻿using invoice.Core.Enums;

namespace invoice.Core.DTO.Shipping
{
    public class ShippingReadDTO
    {
        public bool FromStore { get; set; }
        public bool Delivery { get; set; }
        public string? Region { get; set; }
        public decimal? DeliveryFee { get; set; }

    }
}
