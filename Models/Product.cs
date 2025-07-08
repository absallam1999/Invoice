namespace invoice.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
