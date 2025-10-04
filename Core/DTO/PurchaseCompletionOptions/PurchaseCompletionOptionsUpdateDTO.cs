using System.ComponentModel.DataAnnotations;

namespace invoice.Models.DTO.PurchaseCompletionOptions
{
    public class PurchaseCompletionOptionsUpdateDTO
    {
       
        public bool phone { get; set; }
        public bool Address { get; set; }
        public string? TermsAndConditions { get; set; }
    }
}