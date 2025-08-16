using invoice.Models.Interfaces;
using invoice.Models;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Client
{
    public class ClientInfoDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100,ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Name cannot be empty or whitespace.")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? TextNumber { get; set; }

        
        
    

}
}
