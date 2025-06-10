using System;
using System.Collections.Generic;

namespace Codeshell.Abp.Notifications.Senders
{
    public class NotificationCountSendDto
    {
        public List<Guid> UserIds { get; set; }
    }
}