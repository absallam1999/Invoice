using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Title { get; set; }
        public string Message { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
