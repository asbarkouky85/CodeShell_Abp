using Volo.Abp.Modularity;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Codeshell.Abp.Notifications.Pushing;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellApplicationContractsModule),
        typeof(CodeshellWebModule))]
    public class CodeshellNotificationsWebModule : AbpModule
    {
        public override void RegisterServices(ServiceConfigurationContext context)
        {
            context.Services.AddSignalR();
            context.Services.AddTransient<IDeviceService, NullDeviceService>();
            context.Services.AddSignalRHub<INotificationsPushingContract, NotificationsHub>();
            
        }

        public override void Configure(CodeshellApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationsHub>("/hubs/notificationsHub");
            });
        }
    }
}
