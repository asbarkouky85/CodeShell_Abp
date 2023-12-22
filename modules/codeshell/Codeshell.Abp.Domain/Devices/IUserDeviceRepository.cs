using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Devices
{
    public interface IUserDeviceRepository : IRepository<UserDevice, Guid>
    {
        Task<List<UserDevice>> GetDevices(DevicesRequest req);
        Task<UserDevice> GetByDeviceId(string deviceId);

    }
}
