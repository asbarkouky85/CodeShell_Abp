using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Codeshell.Abp
{
    [DependsOn(
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule)
        )]
    public class CodeshellEntityFrameworkCoreModule : AbpModule
    {
    }
}
