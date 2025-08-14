using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Store
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }    //Slug 
        public string Logo { get; set; }
        public string CoverImage { get; set; }
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

        public string PurchaseCompletionOptionsId { get; set; }
        public PurchaseCompletionOptions PurchaseCompletionOptions { get; set; }
        public ContactInformation ContactInformations { get; set; }
        public Shipping Shipping { get; set; }
        
       // public ICollection<Product> Products { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Page> Pages { get; set; }

    }
}
