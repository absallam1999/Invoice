using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.InvoiceItem
{
    public class UpdateInvoiceItemDTO : InvoiceItemDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }
    }
}
