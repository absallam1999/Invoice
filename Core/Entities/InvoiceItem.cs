using System.ComponentModel.DataAnnotations;

namespace invoice.Core.Entities
{
    public class InvoiceItem : BaseEntity
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; private set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}