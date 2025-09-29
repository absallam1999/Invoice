using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatus
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "pending")]
        Pending = 1,

        [EnumMember(Value = "completed")]
        Completed = 2,

        [EnumMember(Value = "failed")]
        Failed = 3,

        [EnumMember(Value = "refunded")]
        Refunded = 4,

        [EnumMember(Value = "cancelled")]
        Cancelled = 5,
        
        [EnumMember(Value = "expired")]
        Expired = 6,

        [EnumMember(Value = "not_required")]
        NotRequired = 7
    }
}
