namespace invoice.Core.DTO.ContactInformation
{
    public class ContactInfoCreateDTO
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Location { get; set; }
        public string? Facebook { get; set; }
        public string? WhatsApp { get; set; }
        public string? Instagram { get; set; }

        public string? StoreId { get; set; }
    }
}