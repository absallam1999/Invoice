namespace invoice.Core.Entites
{
    public class Client : BaseEntity
    {
        public string Name { get; set; }
      
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? TextNumber { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<Invoice> Invoices { get; set; } = new();
    }
}
