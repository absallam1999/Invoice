using invoice.Core.Entities.utils;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsCreateDTO
    {
        [Required(ErrorMessage = "Store URL is required")]
        [Url(ErrorMessage = "Please provide a valid URL")]
        public string Url { get; set; }

        public IFormFile? Logo { get; set; }
        public IFormFile? CoverImage { get; set; }

        [Required]
        public string Color { get; set; } 

        [Required]
        public string Currency { get; set; }
        
        [Required]
        public string Country { get; set; }

        [Required]
        public PurchaseCompletionOptions PurchaseOptions { get; set; }
    }
}