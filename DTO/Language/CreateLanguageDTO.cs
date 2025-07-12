using System.ComponentModel.DataAnnotations;
using invoice.Models.Enums;

namespace invoice.DTO.Language
{
    public class CreateLanguageDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public LanguageTarget Target { get; set; }
    }
}
