using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationType
    {
        General = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Success = 4
    }
}
