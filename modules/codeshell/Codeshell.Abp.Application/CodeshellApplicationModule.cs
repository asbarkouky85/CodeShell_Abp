using Codeshell.Abp.Linq;
using Codeshell.Abp.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Codeshell.Abp
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(CodeshellApplicationContractsModule),
        typeof(CodeshellDomainModule),
        typeof(CodeshellDomainSharedModule)
    )]
    public class CodeshellApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<CodeshellApplicationModule>();
            context.Services.AddTransient<IQueryProjector, AutoMapperQueryProjector>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<CodeshellApplicationModule>(validate: false);
            });
        }
    }
}
