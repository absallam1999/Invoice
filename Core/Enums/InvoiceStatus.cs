using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceStatus
    {
        [EnumMember(Value = "draft")]
        Draft = 0,

        [EnumMember(Value = "sent")]
        Sent = 1,

        [EnumMember(Value = "viewed")]
        Viewed = 2,

        [EnumMember(Value = "active")]
        Active = 3,

        [EnumMember(Value = "paid")]
        Paid = 4,

        [EnumMember(Value = "overdue")]
        Overdue = 5,

        [EnumMember(Value = "refunded")]
        Refunded = 6,

        [EnumMember(Value = "cancelled")]
        Cancelled = 7,

        [EnumMember(Value = "failed")]
        Failed = 8
    }
}
