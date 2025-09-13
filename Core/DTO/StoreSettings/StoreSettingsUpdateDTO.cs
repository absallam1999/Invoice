using invoice.Models.Entites.utils;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsUpdateDTO
    {
        public IFormFile Logo { get; set; }
        public IFormFile CoverImage { get; set; }
        public string? Color { get; set; }
        public string? Currency { get; set; }
        public PurchaseCompletionOptions? PurchaseOptions { get; set; }
    }
}
