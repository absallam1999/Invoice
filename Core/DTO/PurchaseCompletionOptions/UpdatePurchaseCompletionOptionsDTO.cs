using System.ComponentModel.DataAnnotations;

namespace invoice.Models.DTO.PurchaseCompletionOptions
{
    public class UpdatePurchaseCompletionOptionsDTO
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }

        public bool SendEmail { get; set; }

        [Required(ErrorMessage = "StoreId is required.")]
        public string StoreId { get; set; }
    }
}
