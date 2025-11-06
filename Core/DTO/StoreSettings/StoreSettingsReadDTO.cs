using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Models.Entities.utils;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsReadDTO
    {
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public PurchaseCompletionOptionsReadDTO PurchaseOptions { get; set; }
    }
}


