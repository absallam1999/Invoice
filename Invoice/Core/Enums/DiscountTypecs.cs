using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscountType
    {
        [EnumMember(Value = "amount")]
        Amount = 0,

        [EnumMember(Value = "percentage")]
        Percentage = 1
    }
}