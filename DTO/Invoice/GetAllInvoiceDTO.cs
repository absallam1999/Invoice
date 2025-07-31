using invoice.Models.Enums;
using invoice.Models;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Invoice
{
    public class GetAllInvoiceDTO
    {
        
        public string InvoiceId { get; set; } 
        public string InvoiceCode { get; set; }
        public DateTime InvoiceCreateAt { get; set; }
        public decimal InvoiceValue { get; set; }
       
        public string InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }

        public string ClientId { get; set; }
        public string ClientName { get; set; }


    



       
    }
}
