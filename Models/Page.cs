namespace invoice.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Infooter { get; set; }
        public string Inheader { get; set; }
        
        public int StoreId { get; set; }
        public Store Store { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
