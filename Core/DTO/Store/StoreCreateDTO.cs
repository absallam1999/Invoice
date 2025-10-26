using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Store
{
    public class StoreCreateDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Slug must contain only letters, " +
            "numbers, dashes or underscores without spaces.")]
        public string Slug { get; set; }
        public IFormFile? Logo { get; set; }

        public string Color { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
    }
}