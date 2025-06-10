using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Notifications.Devices
{
    public class DeviceService : ApplicationService, IDeviceService
    {
        private readonly IUserDeviceRepository _userRepo;

        public DeviceService(IUserDeviceRepository userRepo) : base()
        {
            this._userRepo = userRepo;
        }

        public async Task ClearSingalRConnectionData(SignalRDeviceDataDto data)
        {
            var dev = await _userRepo.GetByDeviceId(data.DeviceId);
            if (dev != null)
            {
                await _userRepo.DeleteAsync(dev);
            }
        }

        public async Task UpdateSignalRConnectionData(SignalRDeviceDataDto data)
        {
            var dev = await _userRepo.GetByDeviceId(data.DeviceId);
            if (dev == null)
            {
                dev = new UserDevice(data.DeviceId, data.UserId, data.TenantId, NotificationProviders.Browser);
                await _userRepo.InsertAsync(dev);
            }
            dev.SetConnectionId(data.ConnectionId);
        }
    }
}
