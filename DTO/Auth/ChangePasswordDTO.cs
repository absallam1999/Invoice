using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Auth
{
    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
