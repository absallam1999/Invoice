using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Notification
{
    public class NotificationCreateDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
