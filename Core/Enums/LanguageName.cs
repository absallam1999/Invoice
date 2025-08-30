using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageName
    {
        None = 0,
        Arabic = 1,
        English = 2
    }
}
