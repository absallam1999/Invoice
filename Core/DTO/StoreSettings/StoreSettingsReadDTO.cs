using invoice.Models.Entites.utils;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsReadDTO
    {
        public string Url { get; set; }
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public PurchaseCompletionOptions PurchaseOptions { get; set; }
    }
}
