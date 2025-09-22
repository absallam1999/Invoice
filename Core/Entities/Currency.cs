using invoice.Core.Entities;

namespace invoice.Core.Entities
{
    public class Currency: BaseEntity
    {
        public string CurrencyCode { get; set; }
        public string Country { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
