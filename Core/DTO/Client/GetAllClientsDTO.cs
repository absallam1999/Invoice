namespace invoice.Core.DTO.Client
{
    public class GetAllClientsDTO
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }

        public int InvoiceCount { get; set; }
    }
}