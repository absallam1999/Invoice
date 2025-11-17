using invoice.Core.Enums;

namespace invoice.Core.DTO.Order
{
    public class OrderReadDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string StoreId { get; set; }
        public string StoreName { get; set; }

        public string ClientId { get; set; }
        public string ClientName { get; set; }

        public string InvoiceId { get; set; }
        public decimal? InvoiceFinalValue { get; set; }

        public List<OrderItemReadDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemReadDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}