using System.ComponentModel.DataAnnotations;

namespace invoice.Models.Entities.utils
{
    public class PurchaseCompletionOptions
    {
        public bool Name { get; set; } = true;
        public bool Email { get; set; } = true;
        public bool phone { get; set; } 
        public bool Address { get; set; } 
        public string? TermsAndConditions { get; set; }
    }
}