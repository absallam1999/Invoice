using invoice.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace invoice.Core.Entites
{
    public class ApplicationUser: IdentityUser, IEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Store> Stores { get;  set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
