using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    public enum InvoiceType
    {
        Online,
        PaymentLink,
        Detailed,
        Cashier
    }
}
