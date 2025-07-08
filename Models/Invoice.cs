namespace invoice.Models
{
    public enum InvoiceStatus
    {
        Pending,
        Active,
        Paid
    }

    public enum InvoiceType 
    { 
        Online,
        InStore
    }


    public class Invoice
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreateAt { get; set; }
        public string TaxNumber { get; set; }

        public double Value { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public InvoiceType InvoiceType { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public Payment Payment { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}