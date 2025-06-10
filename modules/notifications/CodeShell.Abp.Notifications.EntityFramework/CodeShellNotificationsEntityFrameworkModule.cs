using Codeshell.Abp.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Codeshell.Abp.Notifications.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellNotificationsDomainModule))]
    public class CodeshellNotificationsEntityFrameworkModule : AbpModule
    {
        public override void RegisterServices(ServiceConfigurationContext context)
        {
            context.Services.AddScoped<NotificationsUnit>();
            context.Services.AddScoped<INotificationsUnit, NotificationsUnit>();

            context.Services.AddRepositoryFor<Notification, NotificationRepository, INotificationRepository>();
            context.Services.AddRepositoryFor<NotificationType, NotificationTypeRepository, INotificationTypeRepository>();
            context.Services.AddMultiTenantDbMigrationsService<NotificationsDbMigrationService>();
            context.Services.AddCodeshellNotificationsEntityFramework<NotificationsContext>();
        }
    }
}
