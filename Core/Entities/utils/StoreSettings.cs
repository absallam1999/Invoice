   

namespace invoice.Models.Entities.utils
{
    public class StoreSettings
    {
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; } 
        public string Currency { get; set; } 
        public string Country { get; set; } 
        
        public PurchaseCompletionOptions purchaseOptions { get; set; }
    }
}
