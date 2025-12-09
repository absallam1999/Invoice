using System.Text.Json.Serialization;

namespace invoice.Core.DTO.PaymentResponse
{
    public class WebhookPayloadDTO
    {
        [JsonPropertyName("rawPayload")]
        public string RawPayload { get; set; } = string.Empty;

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("receivedAt")]
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("sourceIp")]
        public string SourceIp { get; set; } = string.Empty;

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; } = string.Empty;

        // Constructor for easy initialization
        public WebhookPayloadDTO() { }

        public WebhookPayloadDTO(
            string rawPayload,
            Dictionary<string, string> headers = null,
            string signature = "",
            string sourceIp = "",
            string userAgent = "")
        {
            RawPayload = rawPayload;
            Headers = headers ?? new Dictionary<string, string>();
            Signature = signature;
            SourceIp = sourceIp;
            UserAgent = userAgent;
            ReceivedAt = DateTime.UtcNow;
        }

        // Helper method to get specific header value
        public string GetHeader(string headerName)
        {
            return Headers.TryGetValue(headerName, out var value) ? value : string.Empty;
        }

        // Helper method to check if signature is present
        public bool HasSignature()
        {
            return !string.IsNullOrEmpty(Signature);
        }

        // Helper method to validate required fields
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(RawPayload) && Headers != null;
        }

        // Tab-specific header helpers
        public string TapSignature => GetHeader("X-Tap-Signature");
        public string TapTimestamp => GetHeader("X-Tap-Timestamp");
        public string Authorization => GetHeader("Authorization");
    }
}