using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageName
    {
        [EnumMember(Value = "none")]
        None = 0,

        [EnumMember(Value = "arabic")]
        Arabic = 1,

        [EnumMember(Value = "english")]
        English = 2
    }
}
