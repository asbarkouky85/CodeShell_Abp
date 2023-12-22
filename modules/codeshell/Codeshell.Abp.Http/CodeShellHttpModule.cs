using Codeshell.Abp;
using Codeshell.Abp.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Codeshell.Abp
{
    [DependsOn(
        typeof(CodeshellDomainSharedModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule))
        ]
    public class CodeShellHttpModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IScopedProviderAccessor, HttpContextScopedProviderAccessor>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            CodeshellRoot.RootProvider = app.ApplicationServices;
            
            app.Use((context, next) =>
            {

                var cur = context.RequestServices.GetRequiredService<CurrentCulture>();

                if (context.Request.TryReadHeader("accept-language", out string vals))
                {
                    cur.Name = vals;
                }
                else
                {
                    cur.Name = "ar";
                }

                return next();
            });
        }
    }
}
