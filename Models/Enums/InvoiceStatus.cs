using System.Text.Json.Serialization;

namespace invoice.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum InvoiceStatus
    {
       
        Draft = 0,
        Active = 1,
        Paid = 2,
        Refund = 3
    }
}
