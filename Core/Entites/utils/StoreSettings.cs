using invoice.Models.Entites.utils;

namespace invoice.Core.Entites.utils
{
    public class StoreSettings
    {
        public string Url { get; set; } = null!;
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; } = "#FFFFFF";
        public string Currency { get; set; } = "USD";
        
        public PurchaseCompletionOptions purchaseOptions { get; set; }
    }
}
