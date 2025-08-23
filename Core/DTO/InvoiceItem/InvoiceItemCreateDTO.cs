using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.InvoiceItem
{
    public class InvoiceItemCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        public string? ProductId { get; set; }
    }
}
