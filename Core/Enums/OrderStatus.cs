using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        [EnumMember(Value = "pending")]
        Pending = 0,

        [EnumMember(Value = "completed")]
        Completed = 1,

        [EnumMember(Value = "canceled")]
        Canceled = 2
    }
}
