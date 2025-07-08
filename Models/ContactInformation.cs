namespace invoice.Models
{
    public class ContactInformation
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string location { get; set; }
        public string Facebook { get; set; }
        public string WhatsApp { get; set; }
        public string Instagram { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
