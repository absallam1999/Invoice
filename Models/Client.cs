using invoice.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Client : ISoftDeleteable
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Name { get; set; }
        public DateTime cteateAt { get; set; }
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? TextNumber { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }=false;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
    }
}
