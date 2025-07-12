using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Product
{
    public class UpdateProductDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Image { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string CategoryId { get; set; }

        [Required]
        public string StoreId { get; set; }
    }
}
