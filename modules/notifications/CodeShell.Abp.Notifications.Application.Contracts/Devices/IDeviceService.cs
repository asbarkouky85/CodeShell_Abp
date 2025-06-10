using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Devices
{
    public interface IDeviceService
    {
        Task UpdateSignalRConnectionData(SignalRDeviceDataDto data);
        Task ClearSingalRConnectionData(SignalRDeviceDataDto data);
    }
}
