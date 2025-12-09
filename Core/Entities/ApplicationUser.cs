using invoice.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace invoice.Core.Entities
{
    public class ApplicationUser : IdentityUser, IEntity
    {
        //public string? PaypalEmail { get; set; }
        //public string? StripeAccountId { get; set; }
        //public string? EdfaAccountId { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Store Store { get; set; }
        public Tax Tax { get; set; }
        public Currency Currency { get; set; }
        public string? TabAccountId { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<PaymentLink> PaymentLinks { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}