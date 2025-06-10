using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Pushing
{
    public interface INotificationsPushingContract
    {
        Task NotificationsChanged(int count);
    }
}
