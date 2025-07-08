namespace invoice.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
