using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Auth
{
    public class ForgetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
