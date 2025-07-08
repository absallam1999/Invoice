namespace invoice.Models
{
    public class PaymentLink
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public double Value { get; set; }
        public string PaymentsNumber {  get; set; }
        public string Description {  get; set; }
        public string Message {  get; set; }
        public string Image { get; set; }
        public string Terms { get; set; }
        public bool IsDeleted {  get; set; }

        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
