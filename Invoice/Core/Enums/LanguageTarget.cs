using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageTarget
    {
        [EnumMember(Value = "page")]
        Page = 0,

        [EnumMember(Value = "store")]
        Store = 1,

        [EnumMember(Value = "invoice")]
        Invoice = 2
    }
}
