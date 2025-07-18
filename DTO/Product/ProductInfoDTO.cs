using invoice.Custom_Validation;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Product
{
    public class ProductInfoDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
         [ValidImage(ErrorMessage = "Only image files (.jpg, .png, etc.) are allowed.")]
        public IFormFile? Image { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or more.")]
        public int? Quantity { get; set; }
        public string? CategoryId { get; set; }
        public bool InPOS { get; set; } = false;
        public bool InStoe { get; set; } = false;
    }
}
