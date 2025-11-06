using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Models.Entities.utils;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.StoreSettings
{
    public class StoreSettingsUpdateDTO
    {
        public IFormFile? Logo { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }

        public PurchaseCompletionOptionsUpdateDTO PurchaseOptions { get; set; }
    }
}


