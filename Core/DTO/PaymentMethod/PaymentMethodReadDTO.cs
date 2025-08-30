using invoice.Core.Enums;

namespace invoice.Core.DTO.PaymentMethod
{
    public class PaymentMethodReadDTO
    {
        public string Id { get; set; }
        public PaymentType Name { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }

        //public int PaymentsCount { get; set; }
        //public int PayInvoicesCount { get; set; }
    }
}
