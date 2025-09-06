using invoice.Models.Entites.utils;
using invoice.Core.Enums;

namespace invoice.Core.Entites
{
    public class Store : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool Tax { get; set; } = false;
        public bool IsActivated { get; set; } = true;

        public PaymentType PaymentMethod { get; set; } = PaymentType.Cash;

        public string LanguageId { get; set; } = null!;
        public Language Language { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public Shipping Shipping { get; set; } = null!;
        public StoreSettings StoreSettings { get; set; } = null!;

        public List<Page> Pages { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
        public List<ContactInfo> ContactInformations { get; set; } = new();
    }
}
