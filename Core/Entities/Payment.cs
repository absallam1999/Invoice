using invoice.Core.Enums;
using invoice.Helpers;

﻿namespace invoice.Core.Entities
{
    public class Payment : BaseEntity
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public decimal Cost { get; set; }
        public string Currency { get; set; }
        public string GatewaySessionId { get; set; }

        public PaymentType Type { get; set; } = PaymentType.None;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? ExpiresAt { get; set; } = GetSaudiTime.Now().AddDays(7);

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
