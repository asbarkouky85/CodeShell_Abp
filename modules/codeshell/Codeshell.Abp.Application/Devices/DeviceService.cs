using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Devices
{
    public class DeviceService : ApplicationService, IDeviceService
    {
        private readonly IUserDeviceRepository repository;

        public DeviceService(IUserDeviceRepository repository)
        {
            this.repository = repository;
        }
        public async Task ClearSingalRConnectionData(SignalRDeviceDataDto data)
        {
            var dev = await repository.GetByDeviceId(data.DeviceId);
            if (dev != null)
            {
                await repository.DeleteAsync(dev);
            }
        }

        public async Task UpdateSignalRConnectionData(SignalRDeviceDataDto data)
        {
            var dev = await repository.GetByDeviceId(data.DeviceId);
            if (dev == null)
            {
                dev = new UserDevice(data.DeviceId, data.UserId, data.TenantId, DeviceTypes.Browser);
                await repository.InsertAsync(dev);
            }
            dev.SetConnectionId(data.ConnectionId);
        }
    }
}
