using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Client
{
    public class ClientUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(50)]
        public string? TextNumber { get; set; }
    }
}
