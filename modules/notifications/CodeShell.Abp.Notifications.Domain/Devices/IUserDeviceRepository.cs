using Codeshell.Abp.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Notifications.Devices
{
    public interface IUserDeviceRepository : IRepository<UserDevice, long>
    {
        Task<List<UserDevice>> GetDevices(DevicesRequest req);
        Task<UserDevice> GetByDeviceId(string deviceId);

    }
}
