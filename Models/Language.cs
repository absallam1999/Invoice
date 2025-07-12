using invoice.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Language
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public LanguageTarget Target { get; set; }

        public ICollection<Page> Pages { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
