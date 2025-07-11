﻿using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Page
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Infooter { get; set; }
        public string Inheader { get; set; }
        
        public string StoreId { get; set; }
        public Store Store { get; set; }

        public string LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
