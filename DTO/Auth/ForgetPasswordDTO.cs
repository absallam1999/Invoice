using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Auth
{
    public class ForgetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
