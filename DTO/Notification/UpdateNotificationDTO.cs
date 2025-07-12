using System.ComponentModel.DataAnnotations;
public class UpdateNotificationDTO
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    public string UserId { get; set; }
}
