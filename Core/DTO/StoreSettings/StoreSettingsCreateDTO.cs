using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Models.DTO.PurchaseCompletionOptions;
using invoice.Models.Entities.utils;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsCreateDTO
    {
        [Required(ErrorMessage = "Store URL is required")]
        [Url(ErrorMessage = "Please provide a valid URL")]
        public string Url { get; set; }

        public IFormFile? Logo { get; set; }
        public string? CoverImage { get; set; }

        [Required]
        public string Color { get; set; } 

        [Required]
        public string Currency { get; set; }

        [Required]
        public PurchaseCompletionOptionsCreateDTO PurchaseOptions { get; set; }
    }
}