using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public string StoreId { get; set; }
        public Store Store { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
