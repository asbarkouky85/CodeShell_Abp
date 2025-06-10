using System.Collections.Generic;

namespace Codeshell.Abp.Notifications.Senders
{
    public interface INotificationSenderFactory
    {
        List<INotificationSender> GetSenders(NotificationProviders provider);
    }
}