using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Core.Entities
{
    public class Tax : BaseEntity
    {

        public string TaxNumber { get; set; }
        public string TaxName { get; set; }
        public decimal Value { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [InverseProperty("Tax")]
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();


    }
}