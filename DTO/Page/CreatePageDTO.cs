﻿using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Page
{
    public class CreatePageDTO
    {
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public string Infooter { get; set; }

        public string Inheader { get; set; }

        [Required]
        public string StoreId { get; set; }

        [Required]
        public string LanguageId { get; set; }
    }
}
