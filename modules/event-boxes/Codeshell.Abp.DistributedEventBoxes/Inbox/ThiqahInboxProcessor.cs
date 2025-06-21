using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;

using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.Helpers;
using Codeshell.Abp.DistributedEventBoxes.Inbox;
using Volo.Abp.RabbitMQ;

namespace Codeshell.Abp.DistributedEventBoxes.Inbox;

[ExposeServices(typeof(IInboxProcessor), typeof(IThiqahInboxProcessor))]
public class ThiqahInboxProcessor : IInboxProcessor, IThiqahInboxProcessor, ITransientDependency
{
    private readonly IRabbitMqSerializer serializer;
    private readonly ThiqahEventInboxOptions ManehOptions;

    protected IServiceProvider ServiceProvider { get; }
    protected AbpAsyncTimer Timer { get; }
    protected IDistributedEventBus DistributedEventBus { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    protected IClock Clock { get; }
    protected IEventInbox Inbox { get; private set; }
    protected InboxConfig InboxConfig { get; private set; }
    protected AbpEventBusBoxesOptions EventBusBoxesOptions { get; }

    protected DateTime? LastCleanTime { get; set; }

    protected string DistributedLockName => "AbpInbox_" + InboxConfig.Name;
    public ILogger<ThiqahInboxProcessor> Logger { get; set; }
    protected CancellationTokenSource StoppingTokenSource { get; }
    protected CancellationToken StoppingToken { get; }
    IEventBoxLogger CLogger { get; set; }

    public ThiqahInboxProcessor(
        IServiceProvider serviceProvider,
        AbpAsyncTimer timer,
        IDistributedEventBus distributedEventBus,
        IAbpDistributedLock distributedLock,
        IUnitOfWorkManager unitOfWorkManager,
        IClock clock,
        ILogger<ThiqahInboxProcessor> logger,
        IRabbitMqSerializer serializer, IEventBoxLogger consoleLogger,
        IOptions<ThiqahEventInboxOptions> manehOptions,
        IOptions<AbpEventBusBoxesOptions> eventBusBoxesOptions)
    {
        ServiceProvider = serviceProvider;
        Timer = timer;
        DistributedEventBus = distributedEventBus;
        DistributedLock = distributedLock;
        UnitOfWorkManager = unitOfWorkManager;
        Clock = clock;
        ManehOptions = manehOptions.Value;
        EventBusBoxesOptions = eventBusBoxesOptions.Value;
        Timer.Period = Convert.ToInt32(ManehOptions.PeriodTimeSpan.TotalMilliseconds);
        Timer.Elapsed += TimerOnElapsed;
        Logger = logger;
        this.serializer = serializer;
        StoppingTokenSource = new CancellationTokenSource();
        StoppingToken = StoppingTokenSource.Token;
        CLogger = consoleLogger;
    }

    private async Task TimerOnElapsed(AbpAsyncTimer arg)
    {
        await RunAsync();
    }

    public Task StartAsync(InboxConfig inboxConfig, CancellationToken cancellationToken = default)
    {
        InboxConfig = inboxConfig;
        Inbox = (IEventInbox)ServiceProvider.GetRequiredService(inboxConfig.ImplementationType);
        Timer.Start(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        StoppingTokenSource.Cancel();
        Timer.Stop(cancellationToken);
        StoppingTokenSource.Dispose();
        return Task.CompletedTask;
    }

    protected virtual async Task HandleEvent(IncomingEventInfo waitingEvent)
    {
        var retries = waitingEvent.GetRetryCount();
        try
        {
            using (var uow = UnitOfWorkManager.Begin(isTransactional: true, requiresNew: true))
            {
                using (var sw = SW.Measure())
                {


                    CLogger.Log("HANDLE : \t", waitingEvent.EventData);
                    await DistributedEventBus
                        .AsSupportsEventBoxes()
                        .ProcessFromInboxAsync(waitingEvent, InboxConfig);

                    await uow.CompleteAsync(StoppingToken);
                    CLogger.Log("COMPLETE : \t", waitingEvent.EventData);
                    await Inbox.MarkAsProcessedAsync(waitingEvent.Id);
                    CLogger.Log($"PROCESSED : {sw.Elapsed} \t", waitingEvent.EventData);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Inbox Handling Error ({waitingEvent.EventName})");
            Logger.LogException(ex, LogLevel.Error);
            CLogger.Log("FAILED : \t", waitingEvent.EventData);
            using (var uow2 = UnitOfWorkManager.Begin(isTransactional: true, requiresNew: true))
            {
                var mInbox = ServiceProvider.GetRequiredService<IThiqahEventInbox>();
                if (retries >= ManehOptions.MaxRetryCount)
                {
                    Logger.LogInformation("Recycling retries ");
                    await mInbox.UpdateRetries(waitingEvent.Id, 1, ex.Message);
                }
                else
                {
                    retries++;
                    Logger.LogInformation($"Setting retry count {retries}");
                    await mInbox.UpdateRetries(waitingEvent.Id, retries, ex.Message);
                }
                CLogger.Log("EXTRAUPDATED : \t", waitingEvent.EventData);
                await uow2.CompleteAsync(StoppingToken);
            }
        }
        CLogger.Log("-------------------------------------------");
    }

    protected virtual async Task RunAsync(bool presist = false)
    {
        CLogger.Log("RUNNING INBOX PROCESSOR");
        if (StoppingToken.IsCancellationRequested)
        {
            CLogger.Log("CANCEL REQUESTED");
            return;
        }
        var s = SW.Measure();
        await using (var handle = await DistributedLock.TryAcquireAsync(DistributedLockName, cancellationToken: StoppingToken))
        {

            if (handle != null)
            {
                CLogger.Log($"OBTAINED LOCK : {DistributedLockName} {s.Elapsed}");
                while (true)
                {
                    try
                    {
                        var waitingEvents = await Inbox.GetWaitingEventsAsync(EventBusBoxesOptions.InboxWaitingEventMaxCount, StoppingToken);

                        if (waitingEvents.Count == 0)
                        {
                            CLogger.Log($"NO PENDING EVENTS");
                            break;
                        }

                        CLogger.Log($"Found {waitingEvents.Count} events in the inbox.");
                        if (ManehOptions.ParallelHandling)
                        {
                            await HandleParallelly(waitingEvents);
                        }
                        else
                        {
                            await HandleSequentially(waitingEvents);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading from AbpEventInbox");
                        Logger.LogException(ex);
                        break;
                    }
                }
                CLogger.Log("INBOX PROCESSING COMPLETE");
                await DeleteOldEventsAsync();
            }
            else
            {
                CLogger.Log("Could not obtain the distributed lock: " + DistributedLockName);
                if (presist)
                {
                    CLogger.Log($"Will try again in {EventBusBoxesOptions.DistributedLockWaitDuration}");
                }

                try
                {
                    await Task.Delay(EventBusBoxesOptions.DistributedLockWaitDuration, StoppingToken);
                    if (presist)
                    {
                        await RunAsync(presist);
                    }
                }
                catch (TaskCanceledException)
                {

                }
            }
        }
        CLogger.Log("EXIT LOCK");
    }

    private async Task HandleParallelly(List<IncomingEventInfo> waitingEvents)
    {
        CLogger.Log("HANDLING PARALLEL");
        await Parallel.ForEachAsync(waitingEvents, new ParallelOptions { }, async (waitingEvent, token) =>
        {
            await HandleEvent(waitingEvent);
        });
        CLogger.Log("HANDLING PARALLEL COMPLETE");
    }

    private async Task HandleSequentially(List<IncomingEventInfo> waitingEvents)
    {
        CLogger.Log("HANDLING SEQUENCIAL");
        foreach (var waitingEvent in waitingEvents)
        {
            await HandleEvent(waitingEvent);
        }
        CLogger.Log("HANDLING COMPLETE");
    }

    protected virtual async Task DeleteOldEventsAsync()
    {
        if (LastCleanTime != null && LastCleanTime + EventBusBoxesOptions.CleanOldEventTimeIntervalSpan > Clock.Now)
        {
            //CLogger.Log($"LAST CLEAN TIME {LastCleanTime} NO NEED TO CLEAN");
            return;
        }

        await Inbox.DeleteOldEventsAsync();

        LastCleanTime = Clock.Now;
        //CLogger.Log($"DELETEDED OLD EVENTS");
    }

    public async Task Process()
    {
        await RunAsync(true);
    }
}
