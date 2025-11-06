using invoice.Core.Enums;
using System.Text.Json.Serialization;

namespace invoice.Core.DTO.PaymentResponse
{
    public class PaymentWebhookResponse
    {
        [JsonPropertyName("processed")]
        public bool Processed { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("eventId")]
        public string EventId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("invoiceId")]
        public string InvoiceId { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("customerEmail")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("status")]
        public PaymentStatus Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("paymentType")]
        public PaymentType PaymentType { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("feeAmount")]
        public decimal? FeeAmount { get; set; }

        [JsonPropertyName("netAmount")]
        public decimal? NetAmount { get; set; }

        [JsonPropertyName("gatewayName")]
        public string GatewayName { get; set; }

        [JsonPropertyName("gatewayTransactionId")]
        public string GatewayTransactionId { get; set; }

        [JsonPropertyName("gatewayResponse")]
        public object GatewayResponse { get; set; }

        [JsonPropertyName("isTestMode")]
        public bool IsTestMode { get; set; }

        [JsonPropertyName("isRefunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("isDisputed")]
        public bool IsDisputed { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("success")]
        public bool Success => Processed && string.IsNullOrEmpty(ErrorMessage);

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("retryCount")]
        public int RetryCount { get; set; }

        [JsonPropertyName("rawPayload")]
        public string RawPayload { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("webhookVersion")]
        public string WebhookVersion { get; set; }

        [JsonPropertyName("processedAt")]
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("processingTimeMs")]
        public long ProcessingTimeMs { get; set; }

        [JsonPropertyName("webhookSource")]
        public string WebhookSource { get; set; }

        // Constructor for easy creation
        public PaymentWebhookResponse() { }

        public PaymentWebhookResponse(bool processed, string eventType)
        {
            Processed = processed;
            EventType = eventType;
        }

        // Helper methods
        public void AddMetadata(string key, string value)
        {
            Metadata[key] = value;
        }

        public void SetError(string message, string code = null)
        {
            ErrorMessage = message;
            ErrorCode = code;
            Processed = false;
        }

        public void SetSuccess(string paymentId, PaymentStatus status)
        {
            PaymentId = paymentId;
            Status = status;
            Processed = true;
            ErrorMessage = null;
            ErrorCode = null;
        }
    }
}