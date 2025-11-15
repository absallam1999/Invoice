namespace invoice.Core.DTO.PaymentLink
{
    public class GetAllPaymentLinkDTO
    {
        public string Id { get; set; }
        public decimal Value { get; set; }
        public string Slug { get; set; }
        public bool IsActivated { get; set; }
        public int PaymentsNumber { get; set; }
        public int? MaxPaymentsNumber { get; set; }
        public DateTime? ExpireDate { get; set; } 
    }
}
