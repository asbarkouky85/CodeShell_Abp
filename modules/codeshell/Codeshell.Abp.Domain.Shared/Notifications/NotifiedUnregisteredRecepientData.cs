using System.Collections.Generic;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    public class NotifiedUnregisteredRecepientData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<NotifyAttachmentData> Attachments { get; set; }
    }
}
