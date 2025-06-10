using Codeshell.Abp.Notifications.Providers;
using Volo.Abp.Domain.Entities.Auditing;

namespace Codeshell.Abp.Notifications.Types
{
    public class NotificationTypeNotificationProvider : FullAuditedEntity<long>
    {
        public NotificationProvider NotificationProvider { get; protected set; }
        public NotificationProviders NotificationProviderId { get; protected set; }
        public NotificationType NotificationType { get; protected set; }
        public long NotificationTypeId { get; protected set; }

        public NotificationTypeNotificationProvider()
        {
            Id = Utils.GenerateID();

        }
        public NotificationTypeNotificationProvider(NotificationProviders provider) : this()
        {
            NotificationProviderId = provider;
        }
    }
}
