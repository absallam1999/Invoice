namespace invoice.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public string? MainImage { get; set; }
        public List<string>? Images { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public bool InProductList { get; set; } = true;
        public bool InPOS { get; set; } = true;
        public bool InStore { get; set; } = true;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string? CategoryId { get; set; }
        public Category Category { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}
