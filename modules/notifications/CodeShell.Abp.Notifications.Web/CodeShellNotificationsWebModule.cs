using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Pushing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellApplicationContractsModule),
        typeof(CodeShellHttpModule))]
    public class CodeshellNotificationsWebModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            context.Services.AddSignalR();
            context.Services.AddTransient<IDeviceService, NullDeviceService>();
            context.Services.AddSignalRHub<INotificationsPushingContract, NotificationsHub>();

        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationsHub>("/hubs/notificationsHub");
            });
        }
    }
}
