using invoice.Core.Entities.utils;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsReadDTO
    {
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }

        public PurchaseCompletionOptions PurchaseOptions { get; set; }
    }
}


