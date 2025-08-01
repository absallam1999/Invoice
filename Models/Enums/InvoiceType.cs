namespace invoice.Models.Enums
{
    [Flags]
    public enum InvoiceType
    {
        Online,
        PaymentLink,
        Detailed,
        Cashier
        
    }
}
