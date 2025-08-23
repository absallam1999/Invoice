using invoice.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Notification
{
    public class NotificationUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public NotificationType Type { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
