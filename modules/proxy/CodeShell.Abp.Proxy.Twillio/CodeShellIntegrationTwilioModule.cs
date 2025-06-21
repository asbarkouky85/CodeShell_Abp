using Codeshell.Abp;
using Codeshell.Abp.Notifications.Senders;
using CodeShellCore.Integration.Twilio.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace CodeShellCore.Integration.Twilio
{
    [DependsOn(
        typeof(CodeshellApplicationModule)
        )]
    public class CodeShellIntegrationTwilioModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddOptions<TwilioOptions>();
            context.Services.Configure<TwilioOptions>(configuration.GetSection(TwilioConstants.ConfigKey));

            context.Services.AddTransient<INotificationSender, TwilioNotificationSender>();
            context.Services.AddTransient<ITwilioHttpService, TwilioHttpService>();
        }
    }
}
