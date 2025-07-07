namespace invoice.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }

    }
}
