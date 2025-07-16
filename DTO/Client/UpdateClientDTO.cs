using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Client
{
    public class UpdateClientDTO
    {
        [Required(ErrorMessage = "Id is required.")]
       public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string Notes { get; set; }
        public string TextNumber { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
