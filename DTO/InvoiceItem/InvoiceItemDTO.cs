using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.InvoiceItem
{
    public class InvoiceItemDTO
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

    }
}
