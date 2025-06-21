using Maneh.RabbitMQTest.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Autofac;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;
using Volo.Abp.RabbitMQ;

namespace Maneh.RabbitMQTest
{
    [DependsOn(
        typeof(ManehRabbitMQTestModule),
        typeof(AbpAutofacModule),
        typeof(AbpEventBusRabbitMqModule)
        )]
    public class RabbitMQConsole2Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedEventBusOptions>(op =>
            {
               
            });
        }
    }
}
