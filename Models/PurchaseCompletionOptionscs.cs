using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class PurchaseCompletionOptions
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool SendEmail { get; set; }

        public string StoreId { get; set; }
        public Store Store { get; set; }
    }
}
