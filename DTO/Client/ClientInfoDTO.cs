using invoice.Models.Interfaces;
using invoice.Models;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Client
{
    public class ClientInfoDTO
    {

     
            public string Name { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Address { get; set; }
            public string? Notes { get; set; }
            public string? TextNumber { get; set; }

        
        
    

}
}
