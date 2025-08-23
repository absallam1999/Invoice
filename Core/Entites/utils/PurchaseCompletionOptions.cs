using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models.Entites.utils
{
    public class PurchaseCompletionOptions
    {
        public bool Name { get; set; }=true;
        public bool Email { get; set; }=false;
        public bool phone { get; set; }= false;
        public string? TermsAndConditions { get; set; }
    }
}
