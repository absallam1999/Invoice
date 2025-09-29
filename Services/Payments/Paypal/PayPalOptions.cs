namespace invoice.Services.Payments.Paypal
{
    public class PayPalOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";
        public string WebhookId { get; set; }
    }
}