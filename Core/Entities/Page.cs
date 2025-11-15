namespace invoice.Core.Entities
{
    public class Page : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public bool InFooter { get; set; } = true;
        public bool InHeader { get; set; } = true;

        public string StoreId { get; set; }
        public Store Store { get; set; }
    }
}