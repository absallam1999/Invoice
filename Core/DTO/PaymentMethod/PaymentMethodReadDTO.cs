using invoice.Core.Enums;

namespace invoice.Core.DTO.PaymentMethod
{
    public class PaymentMethodReadDTO
    {
        public string Id { get; set; }
        public PaymentType Name { get; set; }
    }
}
