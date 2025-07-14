namespace invoice.DTO.Payment
{
    public class PaymentDetailsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
