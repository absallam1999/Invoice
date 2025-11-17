namespace invoice.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? MainImage { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = null;
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