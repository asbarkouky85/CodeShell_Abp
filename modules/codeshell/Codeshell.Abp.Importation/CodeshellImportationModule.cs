using Volo.Abp.Modularity;

namespace Codeshell.Abp
{
    [DependsOn(typeof(CodeshellDomainSharedModule))]
    public class CodeshellImportationModule : AbpModule
    {

    }
}
