using invoice.Core.DTO.Product;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Core.DTO.InvoiceItem
{
    public class InvoiceItemReadDTO
    {
       // public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice;

        //public string ProductId { get; set; }
        public GetAllProductDTO Product { get; set; }
    }
}
