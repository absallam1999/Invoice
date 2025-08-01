namespace invoice.Models.Enums
{
    [Flags]
    public enum InvoiceStatus
    {
        Pending ,
        Active,
        Paid,
        Refund
    }
}
