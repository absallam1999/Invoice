namespace invoice.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentLink PaymentLink { get; set; }
    }
}
