using System.ComponentModel.DataAnnotations;

namespace invoice.Models.DTO.PurchaseCompletionOptions
{
    public class CreatePurchaseCompletionOptionsDTO
    {
        public bool SendEmail { get; set; }

        [Required(ErrorMessage = "StoreId is required.")]
        public string StoreId { get; set; }
    }
}
