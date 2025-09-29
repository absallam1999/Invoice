using invoice.Core.DTO.Product;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Core.DTO.InvoiceItem
{
    public class InvoiceItemReadDTO
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice;

        public GetAllProductDTO Product { get; set; }
    }
}