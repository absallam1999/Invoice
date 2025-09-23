using invoice.Core.Enums;

namespace invoice.Core.DTO.Invoice
{
    public class InvoiceSummaryDto
    {
        public int NumberOfInvoices { get; set; }
        public decimal TotalCost { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }

    }
}
