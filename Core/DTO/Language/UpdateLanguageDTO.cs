using System.ComponentModel.DataAnnotations;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Language
{
    public class UpdateLanguageDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public LanguageName Name { get; set; }

        
        public LanguageTarget? Target { get; set; }
    }
}
