using Codeshell.Abp;
using Codeshell.Abp.Notifications.Senders;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Codeshell.Abp.Integration.Firebase;
using Codeshell.Abp.Integration.Firebase.Notification.Senders;

namespace Codeshell.Abp.Integration.Firebase
{
    [DependsOn(
        typeof(CodeshellApplicationModule)
        )]
    public class CodeShellIntegrationFirebaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddTransient<IFirebaseNotificationService, FirebaseNotificationService>();
            context.Services.AddTransient<INotificationSender, FirebaseFlutterNotificationSender>();
            context.Services.AddTransient<INotificationSender, FirebaseNotificationSender>();
            context.Services.AddOptions<FirebaseOptions>();
            context.Services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseConstants.ConfigKey));

        }
    }
}
