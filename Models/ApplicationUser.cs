using Microsoft.AspNetCore.Identity;

namespace invoice.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Store> Stores { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
