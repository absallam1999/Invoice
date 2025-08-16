using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Store
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public string Url { get; set; }
        public string? Logo { get; set; }
        public string? CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public bool Cash { get; set; } = true;
        public bool BankTransfer { get; set; } = false;
        public bool PayPal { get; set; } = false;
        public bool Tax { get; set; } = false;
        public bool Arabic { get; set; } = true;
        public bool English { get; set; } = false;
        public bool IsActivated { get; set; } = true;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public PurchaseCompletionOptions PurchaseCompletionOptions { get; set; }
     
        public Shipping Shipping { get; set; }
        public ContactInfo ContactInfo { get; set; }

        public List<Page> Pages { get; set; } = new List<Page>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<ContactInfo> ContactInformations { get; set; } = new List<ContactInfo>();
    }
}
