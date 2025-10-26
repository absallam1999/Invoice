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

        [EnumMember(Value = "paypal")]
        PayPal = 2,

        [EnumMember(Value = "stripe")]
        Stripe = 3,

        [EnumMember(Value = "tab_payments")]
        TabPayments = 4,

        [EnumMember(Value = "apple_pay")]
        ApplePay = 5,

        [EnumMember(Value = "google_pay")]
        GooglePay = 6,

        [EnumMember(Value = "mada")]
        Mada = 7,

        [EnumMember(Value = "stc_pay")]
        STCPay = 8,

        [EnumMember(Value = "edfa")]
        Edfa = 9,

        [EnumMember(Value = "sadad")]
        Sadad = 10,

        [EnumMember(Value = "bank_transfer")]
        BankTransfer = 11,

        [EnumMember(Value = "debit_card")]
        DebitCard = 12,

        [EnumMember(Value = "credit_card")]
        CreditCard = 13,

        [EnumMember(Value = "delivery")]
        Delivery = 14
    }
}