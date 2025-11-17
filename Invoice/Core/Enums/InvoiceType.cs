using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceType
    {
        [EnumMember(Value = "online")]
        Online = 0,

        [EnumMember(Value = "payment_link")]
        PaymentLink = 1,

        [EnumMember(Value = "detailed")]
        Detailed = 2,

        [EnumMember(Value = "cashier")]
        Cashier = 3
    }
}
