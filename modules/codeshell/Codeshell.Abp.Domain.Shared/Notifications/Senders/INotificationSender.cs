using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Senders
{
    public interface INotificationSender
    {
        NotificationProviders ProviderId { get; }
        Task<MessageDeliveryResult> SendAsync(NotificationMessageDeliveryDto deliveryData);
    }
}
