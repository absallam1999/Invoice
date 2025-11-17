using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class Language : BaseEntity
    {
        public LanguageName Name { get; set; }

        public LanguageTarget Target { get; set; }

        //public List<Page> Pages { get; set; } = new();
        //public List<Store> Stores { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
    }
}