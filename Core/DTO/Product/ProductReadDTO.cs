using invoice.Core.DTO.Invoice;

namespace invoice.Core.DTO.Product
{
    public class ProductReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Url { get; set; }
        public string Description { get; set; }

        public string? MainImage { get; set; }
        public List<string>? Images { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int NumberOfSales { get; set; }
        public decimal TotalOfSales { get; set; }

        public List<GetAllInvoiceDTO>? Invoices { get; set; }
    }
}
