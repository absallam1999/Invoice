using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Shipping
    {
        //[Key]
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool FromStore { get; set; } = true;
        public bool Delivery { get; set; } = false;

        //Delivery fees


        [Key, ForeignKey(nameof(Store))]
        public string StoreId { get; set; }
        public Store Store { get; set; }
     

    }
}
