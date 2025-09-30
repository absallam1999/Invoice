namespace invoice.Core.Entities
{
    public class Commission : BaseEntity
    {
        public decimal Value { get; set; }
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }
        public bool Settled { get; set; } = false;
    }
}
