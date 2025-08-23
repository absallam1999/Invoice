using invoice.Core.Enums;

namespace invoice.Core.DTO.Store
{
    public class StoreUpdateDTO
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }

        public bool? Tax { get; set; }
        public bool? IsActivated { get; set; }

        public PaymentType PaymentMethod { get; set; } = PaymentType.Cash;
        public string? LanguageId { get; set; }
    }
}
