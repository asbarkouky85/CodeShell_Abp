using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Maneh.RabbitMQTest
{
    [DependsOn(typeof(AbpEventBusRabbitMqModule))]
    public class ManehRabbitMQTestModule : AbpModule
    {
    }
}
