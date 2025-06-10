using Microsoft.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications.Devices
{
    public interface IDevicesDbContext
    {
        DbSet<UserDevice> UserDevices { get; set; }
    }
}
