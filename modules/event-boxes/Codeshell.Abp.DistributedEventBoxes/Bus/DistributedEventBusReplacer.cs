using Codeshell.Abp.DistributedEventBoxes.Inbox;
using Codeshell.Abp.DistributedEventBoxes.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Collections;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace Codeshell.Abp.DistributedEventBoxes.Bus
{
    [ExposeServices(typeof(IDistributedEventBus), typeof(RabbitMqDistributedEventBus))]
    public class DistributedEventBusReplacer : RabbitMqDistributedEventBus, IDistributedEventBus
    {
        IEventBoxLogger CLogger;
        IEventBoxesJobAccessor _processorAccessor;
        ICorrelationIdProvider _correlationIdProvider;
        string queueType = "Quorum";
        public DistributedEventBusReplacer(
            IOptions<AbpRabbitMqEventBusOptions> options,
            IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
            IConfiguration configuration,
            IConnectionPool connectionPool,
            IRabbitMqSerializer serializer,
            IServiceScopeFactory serviceScopeFactory,
            IRabbitMqMessageConsumerFactory messageConsumerFactory,
            ICurrentTenant currentTenant,
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IClock clock,
            IEventBoxLogger consoleLogger,
            IEventHandlerInvoker eventHandlerInvoker,
            ILocalEventBus localEventBus,
            ICorrelationIdProvider correlationIdProvider,
            IEventBoxesJobAccessor processorAccessor) : base(
                options,
                connectionPool,
                serializer,
                serviceScopeFactory,
                distributedEventBusOptions,
                messageConsumerFactory,
                currentTenant,
                unitOfWorkManager,
                guidGenerator,
                clock,
                eventHandlerInvoker, localEventBus, correlationIdProvider)
        {
            _processorAccessor = processorAccessor;
            _correlationIdProvider = correlationIdProvider;
            var sec = configuration.GetSection("RabbitMQ:EventBus:QueueType");
            if (sec?.Value != null)
            {
                queueType = sec.Value.ToString();
            }
            CLogger = consoleLogger.SetId("Bus");
        }

        protected override void SubscribeHandlers(ITypeList<IEventHandler> handlers)
        {
            if (Consumer.GetType().GetInterfaces().Contains(typeof(IThiqahRabbitMqMessageConsumer)))
            {
                ((IThiqahRabbitMqMessageConsumer)Consumer).InitializeQueue(queueType == "Quorum");
            }

            Consumer.OnMessageReceived(ProcessEventAsync);

            base.SubscribeHandlers(handlers);
        }

        public async override Task ProcessFromInboxAsync(
           IncomingEventInfo incomingEvent,
           InboxConfig inboxConfig)
        {
            var eventType = EventTypes.GetOrDefault(incomingEvent.EventName);
            if (eventType == null)
            {
                return;
            }

            var eventData = Serializer.Deserialize(incomingEvent.EventData, eventType);
            var exceptions = new List<Exception>();
            using (CorrelationIdProvider.Change(incomingEvent.GetCorrelationId()))
            {
                await TriggerHandlersFromInboxAsync(eventType, eventData, exceptions, inboxConfig);
            }
            if (exceptions.Any())
            {
                ThrowOriginalExceptions(eventType, exceptions);
            }
        }

        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;
            var eventType = EventTypes.GetOrDefault(eventName);
            if (eventType == null)
            {
                return;
            }

            var eventBytes = ea.Body.ToArray();

            CLogger.Log("Recieved", eventBytes);
            if (await AddToInboxAsync(ea.BasicProperties.MessageId, eventName, eventType, eventBytes, _correlationIdProvider.Get()))
            {
                CLogger.Log("Stored", eventBytes);
                _ = RunInboxProcessors();
                return;
            }

            var eventData = Serializer.Deserialize(eventBytes, eventType);

            await TriggerHandlersAsync(eventType, eventData);
        }

        private async Task RunInboxProcessors()
        {
            var procs = _processorAccessor.Processors;
            if (procs.Count == 0)
            {
                CLogger.Log("NO INBOX PROCESSORS FOUND");
            }
            foreach (var proc in procs)
            {
                if (proc.GetType().GetInterfaces().Contains(typeof(IThiqahInboxProcessor)))
                {
                    CLogger.Log($"Calling Processor : {proc.GetType().Name}");
                    await ((IThiqahInboxProcessor)proc).Process();
                }
            }
        }
    }
}
