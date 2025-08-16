using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Page
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public bool Infooter { get; set; }=false;
        public bool Inheader { get; set; }=false;
        
        
        public string StoreId { get; set; }
        public Store Store { get; set; }

        public string LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
