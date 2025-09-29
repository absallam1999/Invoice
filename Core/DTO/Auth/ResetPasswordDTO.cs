using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Auth
{
    public class ResetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
