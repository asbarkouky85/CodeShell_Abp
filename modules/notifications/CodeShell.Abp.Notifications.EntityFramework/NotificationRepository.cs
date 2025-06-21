using Codeshell.Abp.Extensions.Linq;
using Codeshell.Abp.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications
{
    public class NotificationRepository : EfCoreRepository<NotificationsContext, Notification, long>, INotificationRepository
    {
        public NotificationRepository(IDbContextProvider<NotificationsContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<PagedResult<Notification>> GetByUser(Guid id, CodeshellPagedRequest opts)
        {
            var o = opts.GetPagedRequestFor<Notification>();

            var q = await GetQueryableAsync();//;
            if (!string.IsNullOrEmpty(o.Filter))
            {
                q = q.Where(d => d.SenderName.Contains(opts.Filter) || d.NotificationType.Name.Contains(opts.Filter) || d.Parameters.Contains(opts.Filter));
            }
            var qSorted = q.Where(d =>
                d.UserId == id &&
                !d.IsRead &&
                d.NotificationMessages.Any(e => e.NotificationProviderId == NotificationProviders.List)
            ).OrderBy(d => d.IsRead).ThenByDescending(d => d.CreationTime);
            return await qSorted.ToPagedResultForType(o);
        }

        public async Task<PagedResult<NotificationMessage>> GetPendingMessages(CodeshellPagedRequest request)
        {
            var req = request.GetPagedRequestFor<NotificationMessage>();
            var q = (await GetDbContextAsync()).Set<NotificationMessage>().AsQueryable().AsSplitQuery();
            q = q.Include(e => e.Notification.User)
                .Include(e => e.NotificationProvider);
            return await q.Where(e => e.SendingStatusId == NotificationSendingStatus.New)
                .ToPagedResultForType(req);
        }

        public async Task<PagedResult<NotificationMessage>> GetPendingRetryMessages(CodeshellPagedRequest request)
        {
            var req = request.GetPagedRequestFor<NotificationMessage>();
            var q = (await GetDbContextAsync()).Set<NotificationMessage>().AsQueryable().AsSplitQuery();
            q = q.Include(e => e.Notification.User)
                .Include(e => e.NotificationProvider);
            return await q.Where(e => e.SendingStatusId == NotificationSendingStatus.Queued || e.SendingStatusId == NotificationSendingStatus.NoDevices)
                .ToPagedResultForType(req);
        }
    }
}
