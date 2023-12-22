using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace Codeshell.Abp
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(CodeshellDomainSharedModule),
        typeof(AbpSettingManagementDomainModule))]
    public class CodeshellDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            DomainUtils.InitGuidGenerator(context.ServiceProvider);
        }
    }
}
