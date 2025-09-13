using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Order
{
    public class OrderUpdateDTO
    {
        [Required]
        public string Id { get; set; }
        public OrderStatus? OrderStatus { get; set; }

        public string? StoreId { get; set; }
        public string? ClientId { get; set; }
        public string? InvoiceId { get; set; }

        public List<OrderItemUpdateDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemUpdateDTO
    {
        public string? Id { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}