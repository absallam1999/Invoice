namespace invoice.Services.Payments.Stripe
{
    public class StripeOptions
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string WebhookSecret { get; set; }
        public bool TestMode { get; set; } = true;
    }
}