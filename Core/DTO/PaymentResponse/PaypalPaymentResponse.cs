using System.Text.Json;
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

        [JsonPropertyName("purchase_units")]
        public List<PayPalPurchaseUnit>? PurchaseUnits { get; set; }

        [JsonPropertyName("links")]
        public List<PayPalLink>? Links { get; set; }

        [JsonPropertyName("payer")]
        public PayPalPayer? Payer { get; set; }
    }

    public class PayPalPurchaseUnit
    {
        [JsonPropertyName("reference_id")]
        public string? ReferenceId { get; set; }

        [JsonPropertyName("custom_id")]
        public string? CustomId { get; set; }

        [JsonPropertyName("invoice_id")]
        public string? InvoiceId { get; set; }

        [JsonPropertyName("amount")]
        public PayPalAmount? Amount { get; set; }

        [JsonPropertyName("payee")]
        public PayPalPayee? Payee { get; set; }

        [JsonPropertyName("payments")]
        public PayPalPayments? Payments { get; set; }
    }

    public class PayPalPayee
    {
        [JsonPropertyName("email_address")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("merchant_id")]
        public string? MerchantId { get; set; }
    }

    public class PayPalPayments
    {
        [JsonPropertyName("authorizations")]
        public List<PayPalAuthorization>? Authorizations { get; set; }

        [JsonPropertyName("captures")]
        public List<PayPalCapture>? Captures { get; set; }
    }

    // Add this new class for Authorizations
    public class PayPalAuthorization
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("amount")]
        public PayPalAmount? Amount { get; set; }

        [JsonPropertyName("create_time")]
        public string? CreateTime { get; set; }

        [JsonPropertyName("update_time")]
        public string? UpdateTime { get; set; }

        [JsonPropertyName("expiration_time")]
        public string? ExpirationTime { get; set; }
    }

    public class PayPalCapture
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("amount")]
        public PayPalAmount? Amount { get; set; }
    }

    public class PayPalAmount
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("currency_code")]
        public string? CurrencyCode { get; set; }
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

    public class PayPalPayer
    {
        [JsonPropertyName("payer_id")]
        public string? PayerId { get; set; }

        [JsonPropertyName("email_address")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("name")]
        public PayPalPayerName? Name { get; set; }
    }

    public class PayPalPayerName
    {
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }
    }

    public class PayPalWebhookEvent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event_type")]
        public string? EventType { get; set; }

        [JsonPropertyName("resource")]
        public JsonElement? Resource { get; set; }
    }

    public class PayPalWebhookVerificationResponse
    {
        [JsonPropertyName("verification_status")]
        public string? VerificationStatus { get; set; }
    }
}