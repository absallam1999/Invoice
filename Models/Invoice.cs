namespace invoice.Models
{

    public enum InvoiceStatus
    {
        Active,
        paid
    }
    
    public enum InvoiceType
    {
        
        
    }



    public class Invoice
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreateAt { get; set; }
        public string TaxNumber { get; set; }

        public double Value { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Type{ get; set; }
        public bool IsDelete { get; set; }

    }
}
