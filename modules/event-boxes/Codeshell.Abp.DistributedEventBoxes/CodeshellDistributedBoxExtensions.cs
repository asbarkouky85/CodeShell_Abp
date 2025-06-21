using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.Inbox;
using Codeshell.Abp.DistributedEventBoxes.Outbox;

namespace Codeshell.Abp.DistributedEventBoxes
{
    public static class CodeshellDistributedBoxExtensions
    {
        public static int GetRetryCount(this IncomingEventRecord inf)
        {
            if (inf.ExtraProperties.TryGetValue("Retries", out object retries))
            {
                return int.Parse(retries.ToString());
            }
            return 0;
        }

        public static double GetProcessedPastTimeInMinutes(this IncomingEventRecord record)
        {
            if (record.ProcessedTime != null)
                return (DateTime.Now - record.ProcessedTime).Value.TotalMinutes;
            return double.MaxValue;
        }

        public static int GetRetryCount(this IncomingEventInfo inf)
        {
            if (inf.ExtraProperties.TryGetValue("Retries", out object retries))
            {
                return int.Parse(retries.ToString());
            }
            return 0;
        }

        public static void SetRetryCount(this IncomingEventInfo record, int count)
        {
            record.ExtraProperties["Retries"] = count;
        }

        public static void SetError(this IncomingEventInfo record, string error)
        {
            record.ExtraProperties["Error"] = error;
        }

        public static void SetRetryCount(this IncomingEventRecord record, int count)
        {
            record.ExtraProperties["Retries"] = count;
        }

        public static void SetError(this IncomingEventRecord record, string error)
        {
            record.ExtraProperties["Error"] = error;
        }

        public static void ConfigureInboxOutbox<TContext>(this IServiceCollection services, IConfiguration configuration, bool outbox = true, bool inbox = true) where TContext : DbContext, IHasEventInbox, IHasEventOutbox
        {
            var conf = configuration.GetSection(DistributedBoxConstants.ConfigurationKey).Get<CodeshellEventInboxOptions>() ?? new CodeshellEventInboxOptions();
            if (conf.Enable)
            {
                var name = typeof(TContext).Name;
                services.Configure<AbpDistributedEventBusOptions>(options =>
                {

                    if (outbox)
                    {
                        options.Outboxes.Configure(name, config =>
                        {
                            config.UseDbContext<TContext>();
                            //config.IsSendingEnabled = false;

                        });
                    }
                    else
                    {
                        options.Outboxes.Configure(e => e.IsSendingEnabled = false);
                    }
                    if (inbox)
                    {
                        options.Inboxes.Configure(name, config =>
                        {
                            config.UseDbContext<TContext>();
                            //config.IsProcessingEnabled = false;
                        });
                    }
                    else
                    {
                        options.Inboxes.Configure(e => e.IsProcessingEnabled = false);
                    }
                });
            }
            services.AddTransient(typeof(ICodeshellEventInbox), typeof(CodeshellDbContextEventInbox<TContext>));
            services.AddTransient(typeof(IDbContextEventOutbox<>), typeof(CodeshellDbContextEventOutbox<>));
        }
    }
}
