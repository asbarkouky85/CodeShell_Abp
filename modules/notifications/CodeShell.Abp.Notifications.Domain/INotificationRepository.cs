using Codeshell.Abp.Data;
using Codeshell.Abp.Linq;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Notifications
{
    public interface INotificationRepository : IRepository<Notification, long>
    {
        Task<PagedResult<Notification>> GetByUser(Guid id, CodeshellPagedRequest opts);
        Task<PagedResult<NotificationMessage>> GetPendingMessages(CodeshellPagedRequest request);
        Task<PagedResult<NotificationMessage>> GetPendingRetryMessages(CodeshellPagedRequest request);
    }
}
