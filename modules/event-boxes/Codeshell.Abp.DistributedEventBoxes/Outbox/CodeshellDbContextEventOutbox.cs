using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.Inbox;

namespace Codeshell.Abp.DistributedEventBoxes.Outbox
{
    [ExposeServices(typeof(DbContextEventOutbox<>), typeof(IDbContextEventOutbox<>))]
    public class CodeshellDbContextEventOutbox<TDbContext> : DbContextEventOutbox<TDbContext> where TDbContext : IHasEventOutbox
    {

        private readonly IUnitOfWorkManager manager;
        IEventBoxLogger CLogger;
        public CodeshellDbContextEventOutbox(
            IDbContextProvider<TDbContext> dbContextProvider,
            IEventBoxLogger consoleLogger,
            IUnitOfWorkManager manager) : base(dbContextProvider)
        {

            this.manager = manager;
            CLogger = consoleLogger.SetId("OutboxDb");
        }

        public override async Task EnqueueAsync(OutgoingEventInfo outgoingEvent)
        {
            CLogger.Log($"BUS: {outgoingEvent.EventName}");
            await base.EnqueueAsync(outgoingEvent);
            CLogger.Log($"DB: {outgoingEvent.EventName}");
            _ = RunSenders(manager.Current).ConfigureAwait(false);
            CLogger.Log("EXIT THREAD");
        }

        async Task RunSenders(IUnitOfWork unitOfWork)
        {
            using (var sc = manager.Begin(new AbpUnitOfWorkOptions { IsTransactional = false, IsolationLevel = IsolationLevel.Serializable }, true))
            {

                await Task.Run(() =>
                {
                    while (!unitOfWork.IsCompleted)
                    {
                        CLogger.Log("Waiting for save");
                        Thread.Sleep(500);
                    }
                });
                CLogger.Log("SAVE COMPLETE");

                try
                {
                    var accessor = sc.ServiceProvider.GetRequiredService<IEventBoxesJobAccessor>();
                    await accessor.RunOutboxSenders();
                }
                catch (Exception ex)
                {
                    CLogger.Log("RUN SENDERS FAILED");
                    CLogger.Log(ex.Message);
                    var excp = ex.InnerException;
                    while (excp != null)
                    {
                        CLogger.Log(excp.Message);
                        excp = ex.InnerException;
                    }
                }

                await sc.CompleteAsync();
            }


        }
    }
}
