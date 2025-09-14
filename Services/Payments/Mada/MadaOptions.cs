namespace invoice.Services.Payments.Mada
{
    public class MadaOptions
    {
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string SecretKey { get; set; }
        public string BaseUrl { get; set; } = "https://secure.mada.com.sa";
    }
}