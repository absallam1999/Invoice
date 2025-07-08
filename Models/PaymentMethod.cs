namespace invoice.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }
}
