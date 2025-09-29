using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Auth
{
    public class UpdateUserDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        public string UserName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
