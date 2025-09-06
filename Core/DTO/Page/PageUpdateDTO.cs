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

        public bool InFooter { get; set; } = false;
        public bool InHeader { get; set; } = false;

        [Required]
        public string StoreId { get; set; }

        [Required]
        public string LanguageId { get; set; }
    }

    public class PageUpdateRangeRequest
    {
        public List<PageUpdateDTO> Pages { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }
}
