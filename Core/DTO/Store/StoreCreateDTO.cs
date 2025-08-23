using invoice.Core.Enums;

namespace invoice.Core.DTO.Store
{
    public class StoreCreateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool Tax { get; set; } = false;
        public PaymentType PaymentMethod { get; set; } = PaymentType.Cash;

        public string LanguageId { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
