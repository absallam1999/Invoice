using invoice.Core.DTO.Invoice;

namespace invoice.Core.DTO.Product
{
    public class ProductWithInvoicesReadDTO: ProductReadDTO
    {
      
        public bool InPOS { get; set; }
        public bool InStore { get; set; }
        public string? CategoryId { get; set; }
        public int NumberOfSales { get; set; }
        public decimal TotalOfSales { get; set; }

        public List<GetAllInvoiceDTO>? Invoices { get; set; }

    }
}
