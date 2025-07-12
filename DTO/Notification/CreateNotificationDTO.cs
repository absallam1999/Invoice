using System.ComponentModel.DataAnnotations;
public class CreateNotificationDTO
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    public string UserId { get; set; }
}
