namespace invoice.Core.DTO.Order
{
    public class OrderUpdateDTO
    {
        public string Id { get; set; }
        public string OrderStatus { get; set; }

        public List<OrderItemUpdateDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemUpdateDTO
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
