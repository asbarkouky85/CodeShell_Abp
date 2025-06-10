using Volo.Abp.Modularity;
using Codeshell.Abp.Notifications.Devices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellApplicationContractsModule)
        )]
    public class CodeshellNotificationsApplicationContractsModule : AbpModule
    {

    }
}
