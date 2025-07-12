using invoice.DTO.Payment;
using System.ComponentModel.DataAnnotations;

public class UpdatePaymentDTO : CreatePaymentDTO
{
    [Required(ErrorMessage = "Id is required.")]
    public string Id { get; set; }
}