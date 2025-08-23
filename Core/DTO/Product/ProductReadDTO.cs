using invoice.Core.DTO.InvoiceItem;

namespace invoice.Core.DTO.Product
{
    public class ProductReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool InProductList { get; set; }
        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? Url { get; set; }

        public string UserId { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? StoreId { get; set; }
        public string? StoreName { get; set; }

        public IEnumerable<InvoiceItemSummaryDTO> InvoiceItems { get; set; } = new List<InvoiceItemSummaryDTO>();
    }

    public class InvoiceItemSummaryDTO
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }

        public decimal LineTotal { get; private set; }
    }
}
