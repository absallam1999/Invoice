using invoice.Models.Enums;

namespace invoice.DTO.Language
{
    public class LanguageDetailsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public LanguageTarget Target { get; set; }
    }
}
