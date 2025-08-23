using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Store
{
    public class StoreReadDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool Tax { get; set; }
        public bool IsActivated { get; set; }
        public PaymentType PaymentMethod { get; set; }

        public string LanguageId { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public StoreSettingsReadDTO? StoreSettings { get; set; }
        public ShippingReadDTO? Shipping { get; set; }
    }
}
