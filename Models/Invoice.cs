using System.ComponentModel.DataAnnotations;
using invoice.Models.Enums;

namespace invoice.Models
{
    public class Invoice
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Code { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string TaxNumber { get; set; }
        public decimal Value { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal FinalValue { get; set; } 

        //public string Description { get; set; }
        public bool IsDelete { get; set; }=false;

        public InvoiceStatus InvoiceStatus { get; set; }
       // public bool IsPaid { get; set; }=false ;
        public InvoiceType InvoiceType { get; set; }

        public string? TermsConditions { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? StoreId { get; set; }
        public Store Store { get; set; }

        public string? ClientId { get; set; }
        public Client Client { get; set; }

        public string LanguageId { get; set; }
        public Language Language { get; set; }

        public Payment Payment { get; set; }
<<<<<<< HEAD
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
=======
        public PayInvoice PayInvoice { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
>>>>>>> aaee6c2c23865a8eab5cc4ecec885f7b2c3a347c
    }
}