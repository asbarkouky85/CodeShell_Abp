using Volo.Abp.Modularity;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellNotificationsDomainModule))]
    public class CodeshellNotificationsEntityFrameworkModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddMultiTenantDbMigrationsService<NotificationsDbMigrationService>();
            context.Services.AddCodeshellNotificationsEntityFramework<NotificationsContext>();
        }
    }
}
