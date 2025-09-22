using invoice.Core.Enums;

﻿namespace invoice.Core.Entities
{
    public class PaymentLink : BaseEntity
    {
        public decimal Value { get; set; }
        public string Purpose { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentsNumber { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string Terms { get; set; }

        public string CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;

        public string? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}