using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications.Devices
{
    public class UserDeviceRepository<TContext> : EfCoreRepository<TContext, UserDevice, long>, IUserDeviceRepository
        where TContext : IEfCoreDbContext, IDevicesDbContext
    {
        public UserDeviceRepository(IDbContextProvider<TContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<UserDevice> GetByDeviceId(string deviceId)
        {
            var q = await GetQueryableAsync();
            UserDevice userDevice = await q.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
            return userDevice;
        }

        public virtual async Task<List<UserDevice>> GetDevices(DevicesRequest req)
        {
            var q = await GetQueryableAsync();

            if (req.LoggedInDevicesOnly)
            {
                q = q.Where(e => e.IsLoggedIn);
            }

            if (req.DeviceType != null)
            {
                if (req.DeviceType == NotificationProviders.List)
                    req.DeviceType = NotificationProviders.Browser;

                q = q.Where(w => w.DeviceTypeId == req.DeviceType);
            }
            if (req.UserId != null)
                q = q.Where(e => e.UserId == req.UserId);
            if (req.UserIds != null)
                q = q.Where(e => req.UserIds.Contains(e.UserId));

            return await q.ToListAsync();
        }
    }
}
