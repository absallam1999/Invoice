using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Page
{
    public class PageUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
        public bool InFooter { get; set; } = false;
        public bool InHeader { get; set; } = false;
    }
}