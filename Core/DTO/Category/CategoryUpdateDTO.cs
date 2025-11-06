using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Category
{
    public class CategoryUpdateDTO
    {

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}