namespace invoice.Services.Payments.Edfa
{
    public class EdfaOptions
    {
        public string ApiKey { get; set; }
        public string MerchantId { get; set; }
        public string WebhookSecret { get; set; }
        public string BaseUrl { get; set; } = "https://api.edfa.com/";
        public bool TestMode { get; set; } = true;
    }

    public class EdfaPaymentLinkResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EdfaPaymentLinkData Data { get; set; }
    }

    public class EdfaPaymentLinkData
    {
        public string PaymentUrl { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class EdfaPaymentStatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EdfaPaymentStatusData Data { get; set; }
    }

    public class EdfaPaymentStatusData
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class EdfaWebhookData
    {
        public string EventId { get; set; }
        public string EventType { get; set; }
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class EdfaBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class EdfaErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}