using System.Text.Json.Serialization;

namespace invoice.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum InvoiceType
    {
        Draft = 0,
        Online=1,
        PaymentLink=2,
        Detailed=3,
        Cashier=4
        
    }
}
