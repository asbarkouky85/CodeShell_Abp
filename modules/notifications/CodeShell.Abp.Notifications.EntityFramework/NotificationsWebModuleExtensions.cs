using Codeshell.Abp.Notifications.Devices;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Notifications
{
    public static class NotificationsEntityFrameworkModuleExtensions
    {
        public static void AddCodeshellNotificationsEntityFramework<TDbContext>(this IServiceCollection coll) where TDbContext : CodeshellDbContext<TDbContext>, IDevicesDbContext
        {
            coll.AddTransient<IUserDeviceRepository, UserDeviceRepository<TDbContext>>();
            coll.AddScoped<IDevicesDbContext, TDbContext>();
        }

    }
}
