using invoice.Models.Entities.utils;
using invoice.Core.Enums;
using invoice.Core.Entities.utils;

namespace invoice.Core.Entities
{
    public class Store : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Slug { get; set; } = null!;
        public bool IsActivated { get; set; } = true;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public Shipping Shipping { get; set; }
        public StoreSettings StoreSettings { get; set; } = null!;
        public ContactInfo ContactInformations { get; set; }

        public PaymentOptions PaymentOptions { get; set; } = null!;
        public ParallelMergeOptions ParallelMergeOptions { get; set; }
        public ICollection<Order> Orders { get; set; }


        public List<Page> Pages { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}