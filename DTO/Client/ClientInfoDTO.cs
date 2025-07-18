using invoice.Models.Interfaces;
using invoice.Models;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Client
{
    public class ClientInfoDTO
    {

        [MaxLength(100,ErrorMessage = "name is required.")]
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
