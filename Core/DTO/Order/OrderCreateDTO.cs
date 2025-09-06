namespace invoice.Core.DTO.Order
{
    public class OrderCreateDTO
    {
        public string StoreId { get; set; }
        public string ClientId { get; set; }
        public string? InvoiceId { get; set; }

        public List<OrderItemCreateDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemCreateDTO
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
