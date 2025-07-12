using System.ComponentModel.DataAnnotations;
public class CreateContactInformationDTO
{
    [Required]
    public string Location { get; set; }

    public string Facebook { get; set; }
    public string WhatsApp { get; set; }
    public string Instagram { get; set; }

    [Required]
    public string StoreId { get; set; }
}
