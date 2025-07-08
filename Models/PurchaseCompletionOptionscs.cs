namespace invoice.Models
{
    public class PurchaseCompletionOptions
    {
        public int Id { get; set; }
        public bool SendEmail { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
