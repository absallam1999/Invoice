using invoice.Core.DTO.ContactInformation;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Store
{
    public class StoreUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Slug must contain only letters, " +
          "numbers, dashes or underscores without spaces.")]
        public string Slug { get; set; }
        public bool IsActivated { get; set; }
        public PaymentOptionsDTO PaymentOptions { get; set; }
        public StoreSettingsUpdateDTO StoreSettings { get; set; }
        public ShippingUpdateDTO Shipping { get; set; }
        public ContactInfoUpdateDTO ContactInfo { get; set; }

    }
}