using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Product
{
    public class CreateProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Image { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or more.")]
        public int Quantity { get; set; }

        [Required]
        public string CategoryId { get; set; }

        [Required]
        public string StoreId { get; set; }
    }
}
