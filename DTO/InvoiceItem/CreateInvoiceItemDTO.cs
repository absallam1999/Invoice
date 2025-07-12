using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.InvoiceItem
{
    public class CreateInvoiceItemDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public string InvoiceId { get; set; }
    }
}
