using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.Inbox;


namespace Codeshell.Abp.DistributedEventBoxes.Inbox
{
    [ExposeServices(typeof(IDbContextEventInbox<>), typeof(ICodeshellEventInbox))]
    public class CodeshellDbContextEventInbox<TDbContext> : DbContextEventInbox<TDbContext>, ICodeshellEventInbox where TDbContext : IHasEventInbox
    {
        private readonly CodeshellEventInboxOptions options;

        public CodeshellDbContextEventInbox(
            IDbContextProvider<TDbContext> dbContextProvider,
            IOptions<CodeshellEventInboxOptions> options,
            IClock clock,
            IOptions<AbpEventBusBoxesOptions> eventBusBoxesOptions) : base(dbContextProvider, clock, eventBusBoxesOptions)
        {
            this.options = options.Value;
        }



        public override async Task<List<IncomingEventInfo>> GetWaitingEventsAsync(int maxCount, CancellationToken cancellationToken = default)
        {
            var dbContext = await DbContextProvider.GetDbContextAsync();

            var retryTime = DateTime.Now.AddSeconds(-options.RetryIntervalInSeconds).ToString("yyyy-MM-dd HH:mm:ss");
            var recycleTime = DateTime.Now.AddMinutes(-options.RetryRecycleTimeInMinutes).ToString("yyyy-MM-dd HH:mm:ss");

            #region Original
            //var outgoingEventRecords = await dbContext
            //    .IncomingEvents
            //    .AsNoTracking()
            //    .Where(x => !x.Processed && (x.ProcessedTime == null || x.ProcessedTime < retryTime))
            //    .OrderBy(x => x.CreationTime)
            //    .Take(maxCount)
            //    .ToListAsync(cancellationToken: cancellationToken);
            #endregion

            IQueryable<IncomingEventRecord> query = dbContext.IncomingEvents.FromSqlRaw("SELECT * FROM AbpEventInbox WHERE Processed=0 AND ExtraProperties={0}", "{}");

            var newEvents = await query.CountAsync();

            if (newEvents == 0)
            {
                var queryString = "Select * from AbpEventInbox where Processed = 0" +
                     $" AND ( ProcessedTime < '{retryTime}')" +
                     $" AND ( ExtraProperties not like '%\"Retries\":{options.MaxRetryCount}%' OR ProcessedTime < '{recycleTime}')";
                query = dbContext.IncomingEvents.FromSqlRaw(queryString);
            }

            var outgoingEventRecords = await query
               .OrderBy(x => x.CreationTime)
               .AsNoTracking()
               .Take(maxCount)
               .ToListAsync();

            return outgoingEventRecords
                .Select(e =>
                {
                    var inf = new IncomingEventInfo(e.Id, e.MessageId, e.EventName, e.EventData, e.CreationTime);
                    foreach (var k in e.ExtraProperties)
                        inf.ExtraProperties[k.Key] = k.Value;
                    return inf;
                })
                .ToList();

        }

        public virtual async Task UpdateRetries(Guid id, int retries, string error)
        {
            var dbContext = await DbContextProvider.GetDbContextAsync();
            var incomingEvent = await dbContext.IncomingEvents.FindAsync(id);
            incomingEvent.SetRetryCount(retries);
            incomingEvent.SetError(error);
            incomingEvent.ProcessedTime = DateTime.Now;
            incomingEvent.Processed = false;
        }

        public override async Task DeleteOldEventsAsync()
        {
            var dbContext = await DbContextProvider.GetDbContextAsync();
            var timeToKeepEvents = (Clock.Now - EventBusBoxesOptions.WaitTimeToDeleteProcessedInboxEvents).ToString("yyyy-MM-dd HH:mm:ss");
            await dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM AbpEventInbox where Processed=1 AND CreationTime < '{timeToKeepEvents}'");
        }
    }
}
