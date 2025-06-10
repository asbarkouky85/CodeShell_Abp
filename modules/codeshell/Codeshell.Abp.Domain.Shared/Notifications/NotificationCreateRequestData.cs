using System;
using System.Collections.Generic;
using System.Linq;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    public class NotificationCreateRequestData
    {
        public long NotificationTypeId { get; set; }
        public List<NotifiedUserData> Users { get; set; }
        public List<NotifiedUnregisteredRecepientData> UnregisteredRecepients { get; set; }
        public List<NotifyAttachmentData> Attachments { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public object Parameters { get; set; }

        public void AddUsers(params long[] ids)
        {
            if (Users == null)
                Users = new List<NotifiedUserData>();
            foreach (var id in ids)
            {
                Users.Add(new NotifiedUserData { UserId = id });
            }
        }

        public bool HasAttachments()
        {
            return Attachments != null && Attachments.Any();
        }
    }
}
