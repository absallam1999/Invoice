using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceStatus
    {
        Draft,
        Sent,
        Viewed,
        Active,
        //PartiallyPaid,
        Paid,
        Overdue,
        Refund,
        //PartiallyRefunded,
        Cancelled,
        Failed
    }
}
