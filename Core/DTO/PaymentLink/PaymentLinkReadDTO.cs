namespace invoice.Core.DTO.PaymentLink
{
    public class PaymentLinkReadDTO
    {
        public string Id { get; set; }
        public decimal Value { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public int PaymentsNumber { get; set; }
        public int? MaxUsageCount { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string Terms { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string InvoiceId { get; set; }
    }
}
