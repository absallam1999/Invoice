using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Order
{
    public class OrderCreateDTO
    {
        [Required]
        public string StoreId { get; set; }

        [Required]
        public string ClientId { get; set; }

        public string? InvoiceId { get; set; }

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

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}