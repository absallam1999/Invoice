using invoice.Core.DTO.User;

namespace invoice.Core.DTO.Invoice
{
    public class InvoicewithUserDTO : InvoiceReadDTO
    {
        public UserDTO User { get; set; }

    }
}
