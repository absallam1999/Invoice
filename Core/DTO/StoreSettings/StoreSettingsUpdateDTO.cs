using invoice.Models.Entites.utils;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsUpdateDTO
    {
        [Url(ErrorMessage = "Please provide a valid URL")]
        public string? Url { get; set; }

        public IFormFile Logo { get; set; }
        public IFormFile CoverImage { get; set; }
        public string? Color { get; set; }
        public string? Currency { get; set; }
        public PurchaseCompletionOptions? PurchaseOptions { get; set; }
    }
}
