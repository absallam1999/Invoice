namespace invoice.Services.Payments.ApplePay
{
    public class ApplePayOptions
    {
        public string MerchantIdentifier { get; set; }
        public string DisplayName { get; set; }
        public string MerchantDomain { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}