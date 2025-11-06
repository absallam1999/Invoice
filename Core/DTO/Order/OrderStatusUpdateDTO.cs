using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Order
{
    public class OrderStatusUpdateDTO
    {
        [Required]
        public string OrderId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }
    }

    public class OrderFilterDTO
    {
        public string? ClientId { get; set; }
        public string? StoreId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class OrderSummaryDTO
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}