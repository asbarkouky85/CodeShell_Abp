using Codeshell.Abp.EntityFrameworkCore.Devices;
using Codeshell.Abp.EntityFramework;
using Codeshell.Abp.Extensions.DependencyInjection;
using Codeshell.Abp.Notifications.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Notifications
{
    public static class NotificationsEntityFrameworkModuleExtensions
    {
        public static void AddCodeshellNotificationsEntityFramework<TDbContext>(this IServiceCollection coll) where TDbContext : CodeshellDbContext<TDbContext>, IDevicesDbContext
        {
            coll.AddRepositoryFor<UserDevice, UserDeviceRepository<TDbContext>, IUserDeviceRepository>();
            coll.AddUnitOfWork<CodeshellNotificationsUnit<TDbContext>, ICodeshellNotificationsUnit>();
        }

    }
}
