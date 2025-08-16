using System.Text.Json.Serialization;

namespace invoice.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscountType
    {
        Amount,   
        Percentage 
    }
}
