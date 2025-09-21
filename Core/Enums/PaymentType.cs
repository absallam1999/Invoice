using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
           
        Cash ,
        PayPal,


        DebitCard,
        BankTransfer,
        CreditCard,
        Stripe,
        ApplePay,
        GooglePay,
        Mada,
        STCPay,
        Sadad,
        Delivery



    }
}
