using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications
{
    public interface INotificationCreateService
    {
        Task CreateNotifications(NotificationCreateRequestData request);
    }
}