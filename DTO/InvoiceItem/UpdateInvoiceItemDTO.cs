using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.InvoiceItem
{
    public class UpdateInvoiceItemDTO : CreateInvoiceItemDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }
    }
}
