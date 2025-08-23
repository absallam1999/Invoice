using invoice.Models.Entites.utils;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsCreateDTO
    {
        [Required(ErrorMessage = "Store URL is required")]
        [Url(ErrorMessage = "Please provide a valid URL")]
        public string Url { get; set; }

        public string? Logo { get; set; }
        public string? CoverImage { get; set; }

        [Required]
        public string Color { get; set; } = "#FFFFFF";

        [Required]
        public string Currency { get; set; } = "USD";

        [Required]
        public PurchaseCompletionOptions PurchaseOptions { get; set; }
    }
}
