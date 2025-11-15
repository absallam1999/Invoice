using System.Text.Json.Serialization;

namespace invoice.Core.DTO.PaymentResponse
{
    public class PayPalAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class PayPalOrderResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("links")]
        public List<PayPalLink>? Links { get; set; }
    }

    public class PayPalLink
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("rel")]
        public string? Rel { get; set; }

        [JsonPropertyName("method")]
        public string? Method { get; set; }
    }

    public class PayPalWebhookEvent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event_type")]
        public string? EventType { get; set; }

        [JsonPropertyName("resource")]
        public PayPalWebhookResource? Resource { get; set; }
    }

    public class PayPalWebhookResource
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    public class PayPalWebhookVerificationResponse
    {
        [JsonPropertyName("verification_status")]
        public string? VerificationStatus { get; set; }
    }
}
