using invoice.Core.Enums;

namespace invoice.Core.Entities
{
    public class Notification: BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
