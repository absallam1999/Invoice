using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Product
{
    public class ProductUpdateDTO
    {
        [Required]
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = null;

        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? CategoryId { get; set; }
       
    }
}