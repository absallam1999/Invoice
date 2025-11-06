namespace invoice.Core.DTO.Invoice
{
    public class InvoiceSummaryWithDateDto: InvoiceSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }

    }
}
