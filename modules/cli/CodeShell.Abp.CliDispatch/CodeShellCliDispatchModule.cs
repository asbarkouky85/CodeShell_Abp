using Codeshell.Abp.CliDispatch.Help;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.CliDispatch
{
    [DependsOn(
        typeof(CodeshellApplicationModule)
        )]
    public class CodeShellCliDispatchModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundWorkerOptions>(e => e.IsEnabled = false);
            context.Services.AddOptions<CliDispatchOptions>();
            var builder = context.Services.GetCliRouteBuilder();
            builder.AddHandler<HelpRequestHandler>("help");
        }

    }
}
