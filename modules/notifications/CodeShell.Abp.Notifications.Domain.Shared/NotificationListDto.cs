using FMS.Notifications;
using System;

namespace Codeshell.Abp.Notifications
{

    public class NotificationListDto : INotificationListDto
    {
        public long Id { get; set; }
        public bool IsRead { get; set; }
        public string Body { get; set; }
        public string Resource { get; set; }
        public long? EntityId { get; set; }
        public string SenderName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ReadOn { get; set; }

    }
}
