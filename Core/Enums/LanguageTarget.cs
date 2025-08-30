using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageTarget
    {
        Page,
        Store,
        Invoice
    }
}
