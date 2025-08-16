using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Store
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public bool IsActivated { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<Page> Pages { get; set; } = new List<Page>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<ContactInformation> ContactInformations { get; set; } = new List<ContactInformation>();
        public PurchaseCompletionOptions PurchaseCompletionOptions { get; set; }
    }
}
