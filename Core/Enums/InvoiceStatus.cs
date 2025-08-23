using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    public enum InvoiceStatus
    {
        Draft = 0,
        Sent = 1,
        Viewed = 2,
        Active = 3,
        PartiallyPaid = 4,
        Paid = 5,
        Overdue = 6,
        Refund = 7,
        PartiallyRefunded = 8,
        Cancelled = 9,
        Failed = 10 
    }
}
