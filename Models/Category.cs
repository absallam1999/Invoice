using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Category
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Name { get; set; }
<<<<<<< HEAD
        public List<Product> Products { get; set; } = new List<Product>();
=======
        public string UserId { get; set; }
        //delete

        public ICollection<Product> Products { get; set; }
>>>>>>> aaee6c2c23865a8eab5cc4ecec885f7b2c3a347c
    }
}
