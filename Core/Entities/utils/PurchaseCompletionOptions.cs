using System.ComponentModel.DataAnnotations;

namespace invoice.Models.Entities.utils
{
    public class PurchaseCompletionOptions
    {
        public bool Name { get; set; } 
        public bool Email { get; set; } 
        public bool phone { get; set; } 
        public bool Address { get; set; } 
        public string? TermsAndConditions { get; set; }
    }
}