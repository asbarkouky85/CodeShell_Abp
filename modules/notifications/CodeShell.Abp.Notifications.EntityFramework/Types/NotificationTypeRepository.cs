using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Codeshell.Abp.Notifications;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications.Types
{
    public class NotificationTypeRepository : EfCoreRepository<NotificationsContext, NotificationType, long>, INotificationTypeRepository
    {
        public NotificationTypeRepository(IDbContextProvider<NotificationsContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<NotificationType>> GetWithTemplates(List<long> types)
        {
            var q = await GetQueryableAsync();
            return await q.Include(e => e.Templates).Where(e => types.Contains(e.Id)).ToListAsync();
        }

        public async Task<NotificationType> GetWithProviders(long id)
        {
            var q = await GetQueryableAsync();
            q = q.Include(e => e.Providers);
            return await q.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<NotificationType> GetWithTemplates(long notificationTypeId)
        {
            var q = await GetQueryableAsync();
            return await q.Include(e => e.Templates).Where(e => e.Id == notificationTypeId).FirstOrDefaultAsync();
        }
    }
}
