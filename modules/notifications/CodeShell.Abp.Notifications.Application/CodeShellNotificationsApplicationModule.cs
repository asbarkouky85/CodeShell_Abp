using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Senders;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellNotificationsApplicationContractsModule),
        typeof(CodeshellApplicationModule),
        typeof(CodeshellNotificationsDomainModule)
        )]
    public class CodeshellNotificationsApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IDeviceService, DeviceService>();

            context.Services.AddTransient<INotificationsListService, NotificationListService>();
            context.Services.AddTransient<INotificationCreateService, NotificationCreateService>();
            context.Services.AddTransient<INotificationDeliveryService, NotificationDeliveryService>();
            context.Services.AddTransient<INotificationSender, ListNotificationSender>();
            context.Services.AddTransient<INotificationSender, EmailNotificationSender>();
            context.Services.AddTransient<INotificationSenderFactory, NotificationSenderFactory>();

            context.Services.AddTransient<IListNotificationSender, ListNotificationSender>();

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(async () =>
            {
                await context.ServiceProvider.GetRequiredService<INotificationDeliveryService>().SendPendingMessages();
            });
        }
    }
}
