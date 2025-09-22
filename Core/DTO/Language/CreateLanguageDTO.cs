using System.ComponentModel.DataAnnotations;
using invoice.Core.Enums;

namespace invoice.Core.DTO.Language
{
    public class CreateLanguageDTO
    {
        [Required]
        public LanguageName Name { get; set; }

        [Required]
        public LanguageTarget? Target { get; set; }
    }
}