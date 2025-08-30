using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        None = 0,   
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        BankTransfer = 4,
        PayPal = 5,
        Stripe = 6,
        ApplePay = 7,
        GooglePay = 8,
        Mada = 9,
        STCPay = 10,
        Sadad = 11,
        Delivery = 12
    }
}
