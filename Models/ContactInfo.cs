using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models
{
    public class ContactInfo
    {
        //[Key]
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? location { get; set; }
        public string? Facebook { get; set; }
        public string? WhatsApp { get; set; }
        public string Instagram { get; set; }
        [Key, ForeignKey(nameof(Store))]
        public string? StoreId { get; set; }
        public Store Store { get; set; }
       
    }
}
