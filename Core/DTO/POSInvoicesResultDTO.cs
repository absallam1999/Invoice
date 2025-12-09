using invoice.Core.DTO.Invoice;

namespace invoice.Core.DTO
{
    public class POSInvoicesResultDTO
    {
        public IEnumerable<GetAllInvoiceDTO> Invoices { get; set; } = new List<GetAllInvoiceDTO>();

        public decimal TotalValue { get; set; }

    }
}
