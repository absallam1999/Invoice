using invoice.Models.Enums;
using invoice.Models;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Invoice
{
    public class GetAllInvoiceDTO
    {
        
        public string InvoiceId { get; set; } 
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceCreateAt { get; set; }
        public double InvoiceValue { get; set; }
       
        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }


    



       
    }
}
