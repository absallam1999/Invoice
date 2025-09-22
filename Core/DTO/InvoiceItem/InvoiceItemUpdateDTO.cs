using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.InvoiceItem
{
    public class InvoiceItemUpdateDTO
    {
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        public string ProductId { get; set; }
    }
}