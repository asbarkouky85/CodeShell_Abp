using Codeshell.Abp.Lookups;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Types;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Codeshell.Abp.Notifications.Providers
{
    public class NotificationProvider : FullAuditedEntity<NotificationProviders>, INamed<NotificationProviders>
    {
        public string Name { get; set; }
        public bool SendDirectly { get; set; }
        public bool EnableRetry { get; set; }
        public bool RequiresDevices { get; set; }
        public ICollection<NotificationTypeNotificationProvider> NotificationTypes { get; set; }
        public ICollection<NotificationTypeTemplate> Templates { get; set; }
        public ICollection<UserDevice> UserDevices { get; set; }

        private NotificationProvider()
        {
            NotificationTypes = new HashSet<NotificationTypeNotificationProvider>();
            Templates = new HashSet<NotificationTypeTemplate>();
            UserDevices = new HashSet<UserDevice>();

        }

        public NotificationProvider(NotificationProviders id, bool requiresDevices = false, string name = null) : this()
        {
            Id = id;
            Name = name ?? id.ToString();
            RequiresDevices = requiresDevices;
        }
    }
}
