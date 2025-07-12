using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Store
{
    public class CreateStoreDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Store name must be less than 100 characters.")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "URL must be valid.")]
        public string Url { get; set; }
        public string Logo { get; set; }
        public string CoverImage { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Color must be a valid hex.")]
        public string Color { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Currency code must be less than 10 characters.")]
        public string Currency { get; set; }
        public bool IsActivated { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }
    }
}
