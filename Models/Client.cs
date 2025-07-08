namespace invoice.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } 
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string TextNumber { get; set; }
        public bool IsDeleted { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
    }
}
