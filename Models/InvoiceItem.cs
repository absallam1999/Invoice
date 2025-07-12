using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class InvoiceItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
