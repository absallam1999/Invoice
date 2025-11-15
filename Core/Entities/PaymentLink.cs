using invoice.Core.Entities.utils;
using invoice.Core.Enums;
using invoice.Models.Entities.utils;

﻿namespace invoice.Core.Entities
{
    public class PaymentLink : BaseEntity
    {
        public decimal Value { get; set; }
        public string Slug { get; set; } = null!;
        public bool IsActivated { get; set; } = true;
        public int PaymentsNumber { get; set; } = 0;
        public DateTime? ExpireDate { get; set; } = null;
        public int? MaxPaymentsNumber { get; set; } = null ;
       public string? Description { get; set; }
        public string Currency { get; set; }



        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public PaymentOptions PaymentOptions { get; set; } = null!;
        public PurchaseCompletionOptions purchaseOptions { get; set; }

        public ICollection<PaymentLinkPayments> PaymentLinkPayments { get; set; }


    }
}