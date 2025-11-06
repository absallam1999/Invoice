using invoice.Core.DTO.Currency;
using invoice.Core.DTO.Tax;
using invoice.Core.DTO.User;

namespace invoice.Core.DTO.Invoice
{
    public class InvoicewithUserDTO : InvoiceReadDTO
    {
        public UserDTO User { get; set; }
        public CurrencyReadDTO? CurrencyInfo { get; set; }



    }
}