using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Client
{
    public class ClientCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        [RegularExpression(@"^\S+$", ErrorMessage = "Email must not contain spaces")]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

    }
}