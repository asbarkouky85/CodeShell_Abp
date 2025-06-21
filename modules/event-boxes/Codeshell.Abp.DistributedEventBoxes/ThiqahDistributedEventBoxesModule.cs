using Medallion.Threading;
using Medallion.Threading.Redis;
using Medallion.Threading.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.DistributedLock;
using Codeshell.Abp.DistributedEventBoxes.Inbox;

namespace Codeshell.Abp.DistributedEventBoxes
{
    [DependsOn(
        typeof(AbpEventBusRabbitMqModule),
        typeof(AbpDistributedLockingModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class ThiqahDistributedEventBoxesModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var configuration = context.Services.GetConfiguration();
            Configure<AbpEventBusBoxesOptions>(op =>
            {
                op.DistributedLockWaitDuration = new TimeSpan(0, 0, 5);
                op.InboxWaitingEventMaxCount = 100;
                op.CleanOldEventTimeIntervalSpan = new TimeSpan(48, 0, 0);
                
            });

            context.Services.AddTransient(typeof(IDbContextEventInbox<>), typeof(ThiqahDbContextEventInbox<>));

            context.Services.AddOptions<ThiqahEventInboxOptions>(DistributedBoxConstants.ConfigurationKey);
            context.Services.Configure<ThiqahEventInboxOptions>(configuration.GetSection(DistributedBoxConstants.ConfigurationKey));

            context.Services.AddSingleton<IDistributedLockProvider>(sp =>
            {
                //var opts = sp.GetRequiredService<IOptions<ThiqahEventInboxOptions>>().Value;
                var conf = sp.GetRequiredService<IConfiguration>();
                var opts = conf.GetSection(DistributedBoxConstants.ConfigurationKey).Get<ThiqahEventInboxOptions>() ?? new ThiqahEventInboxOptions();

                switch (opts.LockType)
                {
                    case DistributedLockTypes.Redis:
                        var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                        return new RedisDistributedSynchronizationProvider(connection.GetDatabase());

                    default:

                        var conn = conf.GetConnectionString("Default");
                        return new SqlDistributedSynchronizationProvider(conn);
                }

            });
            
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            
            _ = _runEventBoxes(context.ServiceProvider).ConfigureAwait(false);
        }

        private async Task _runEventBoxes(IServiceProvider provider)
        {
            using (var sc = provider.CreateScope())
            {
                var accessor = sc.ServiceProvider.GetRequiredService<IEventBoxesJobAccessor>();
                await accessor.RunOutboxSenders();
                await accessor.RunInboxProcessors();
            }

        }
    }
}
