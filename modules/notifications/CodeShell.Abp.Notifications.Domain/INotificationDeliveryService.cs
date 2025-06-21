using System.Collections.Generic;
using System.Threading.Tasks;
using Codeshell.Abp.Notifications.Senders;

namespace Codeshell.Abp.Notifications
{
    public interface INotificationDeliveryService
    {
        Task SendPendingMessages();
        Task RetryFailedMessages();

        Task SendMessages(IEnumerable<NotificationMessage> messages);
        Task<MessageDeliveryResult> SendMessage(NotificationMessage message);
    }
}
