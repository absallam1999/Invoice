namespace invoice.Models
{
    public class Store
    {
        public int Id { get; set; }
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

        public ICollection<Product> Products { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ICollection<ContactInformation> ContactInformations { get; set; }
        public PurchaseCompletionOptions PurchaseCompletionOptions { get; set; }
    }
}
