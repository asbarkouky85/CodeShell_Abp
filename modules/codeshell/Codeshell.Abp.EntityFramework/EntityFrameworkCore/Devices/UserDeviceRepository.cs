using Codeshell.Abp.Devices;
using Codeshell.Abp.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.EntityFrameworkCore.Devices
{
    public abstract class UserDeviceRepositoryBase<TContext> : EfCoreRepository<TContext, UserDevice, Guid>, IUserDeviceRepository
        where TContext : DbContext, IDevicesDbContext
    {
        protected UserDeviceRepositoryBase(IDbContextProvider<TContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<UserDevice> GetByDeviceId(string deviceId)
        {
            var q = (await GetDbSetAsync()).AsQueryable();
            UserDevice? userDevice = await q.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
            return userDevice;
        }

        public virtual async Task<List<UserDevice>> GetDevices(DevicesRequest req)
        {
            var q = (await GetDbSetAsync()).AsQueryable();
            if (req.DeviceType != null)
                q = q.Where(w => w.DeviceTypeId == req.DeviceType);
            if (req.UserId != null)
                q = q.Where(e => e.UserId == req.UserId);
            if (req.UserIds != null)
                q = q.Where(e => req.UserIds.Contains(e.UserId));

            return await q.ToListAsync();
        }
    }
}
