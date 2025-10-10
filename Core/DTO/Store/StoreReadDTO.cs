using invoice.Core.DTO.ContactInformation;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.DTO.Tax;
using invoice.Core.Entities.utils;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Store
{
    public class StoreReadDTO
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Slug { get; set; }
        public bool IsActivated { get; set; }
        public PaymentOptionsDTO PaymentOptions { get; set; }
        public StoreSettingsReadDTO? StoreSettings { get; set; }
        public ShippingReadDTO? Shipping { get; set; }
        public ContactInfoReadDTO? ContactInfo { get; set; }
        public TaxReadDTO? Tax { get; set; }
    }
}