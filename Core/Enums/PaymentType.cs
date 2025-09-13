using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        [EnumMember(Value = "none")]
        None = 0,

        [EnumMember(Value = "cash")]
        Cash = 1,

        [EnumMember(Value = "credit_card")]
        CreditCard = 2,

        [EnumMember(Value = "debit_card")]
        DebitCard = 3,

        [EnumMember(Value = "bank_transfer")]
        BankTransfer = 4,

        [EnumMember(Value = "paypal")]
        PayPal = 5,

        [EnumMember(Value = "stripe")]
        Stripe = 6,

        [EnumMember(Value = "apple_pay")]
        ApplePay = 7,

        [EnumMember(Value = "google_pay")]
        GooglePay = 8,

        [EnumMember(Value = "mada")]
        Mada = 9,

        [EnumMember(Value = "stc_pay")]
        STCPay = 10,

        [EnumMember(Value = "sadad")]
        Sadad = 11,

        [EnumMember(Value = "delivery")]
        Delivery = 12
    }
}
