using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Invoice
{
    public class UpdateInvoiceDTO : CreateInvoiceDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }
    }
}
