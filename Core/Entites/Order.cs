using invoice.Core.Enums;

namespace invoice.Core.Entites
{
    public class Order : BaseEntity
    {
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }

        public string StoreId { get; set; }
        public Store Store { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string ClientId { get; set; }
        public Client Client { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
