using Codeshell.Abp.Cli.Help;
using Codeshell.Abp.Cli.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Cli
{
    [DependsOn(
        typeof(CodeshellApplicationModule),
        typeof(AbpAutofacModule)
        )]
    public class CodeshellCliModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundWorkerOptions>(e => e.IsEnabled = false);           
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var coll = context.ServiceProvider.GetRequiredService<ICliRouteBuilder>();
            coll.AddHandler<HelpCliHandler>("help");
        }
    }
}
