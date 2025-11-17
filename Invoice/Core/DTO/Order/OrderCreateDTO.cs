using System.ComponentModel.DataAnnotations;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Order
{
    public class OrderCreateDTO
    {
        [Required] public string Currency { get; set; }
        [Required] public string StoreId { get; set; }
        [Required] public string ClientId { get; set; }

        public string? LanguageId { get; set; }
        public bool Tax { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemCreateDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemCreateDTO
    {
        [Required]
        public string ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
