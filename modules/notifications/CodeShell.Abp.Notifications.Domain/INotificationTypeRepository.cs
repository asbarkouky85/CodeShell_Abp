using Codeshell.Abp.Data;
using Codeshell.Abp.Notifications.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Notifications
{
    public interface INotificationTypeRepository : IRepository<NotificationType, long>
    {
        Task<List<NotificationType>> GetWithTemplates(List<long> types);
        Task<NotificationType> GetWithProviders(long id);
        Task<NotificationType> GetWithTemplates(long notificationTypeId);
    }
}