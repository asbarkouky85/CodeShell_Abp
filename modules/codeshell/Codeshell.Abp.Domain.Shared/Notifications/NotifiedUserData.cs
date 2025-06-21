using System;
using System.Collections.Generic;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    public class NotifiedUserData
    {
        public Guid UserId { get; set; }
        public List<NotifyAttachmentData> Attachments { get; set; }

        public bool HasAttachments()
        {
            return Attachments != null && Attachments.Count > 0;
        }
    }
}
