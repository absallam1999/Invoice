using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationType
    {
        [EnumMember(Value = "general")]
        General = 0,

        [EnumMember(Value = "info")]
        Info = 1,

        [EnumMember(Value = "warning")]
        Warning = 2,

        [EnumMember(Value = "error")]
        Error = 3,

        [EnumMember(Value = "success")]
        Success = 4
    }
}