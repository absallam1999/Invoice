using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models
{
    public class PurchaseCompletionOptions
    {
        //[Key]
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool Name { get; set; }=true;
        public bool Email { get; set; }=false;
        public bool phone { get; set; }= false;
        public string? TermsAndConditions { get; set; }
        [Key, ForeignKey(nameof(Store))]
        public string StoreId { get; set; }
        public Store Store { get; set; }
    }
}
