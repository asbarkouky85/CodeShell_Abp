using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;
using Codeshell.Abp.DistributedEventBoxes;
using Codeshell.Abp.DistributedEventBoxes.Inbox;
using Codeshell.Abp.DistributedEventBoxes.Outbox;

namespace Codeshell.Abp.DistributedEventBoxes.Inbox
{
    public class EventBoxesJobAccessor : IEventBoxesJobAccessor
    {
        private readonly IServiceProvider provider;
        IEventBoxLogger CLogger;
        public EventBoxesJobAccessor(IServiceProvider provider)
        {
            this.provider = provider;
            CLogger = provider.GetService<IEventBoxLogger>().SetId("Outbox");

        }

        public List<IInboxProcessor> Processors
        {
            get
            {
                var procs = new List<IInboxProcessor>();
                var inst = provider.GetService<InboxProcessManager>();
                if (inst != null)
                {
                    var prop = typeof(InboxProcessManager).GetProperty("Processors", BindingFlags.Instance | BindingFlags.NonPublic);
                    procs = (List<IInboxProcessor>)prop.GetValue(inst);
                }

                return procs;

            }
        }

        public List<IOutboxSender> Senders
        {
            get
            {
                var procs = new List<IOutboxSender>();

                try
                {
                var outboxSenderManager = provider.GetService<OutboxSenderManager>();
                if (outboxSenderManager != null)
                {
                    var prop = typeof(OutboxSenderManager).GetProperty("Senders", BindingFlags.Instance | BindingFlags.NonPublic);
                    procs = (List<IOutboxSender>)prop.GetValue(outboxSenderManager);
                }
                }
                catch (Exception ex)
                {

                }

                return procs;
            }
        }

        public async Task RunInboxProcessors()
        {
            var procs = Processors;
            foreach (var proc in procs)
            {
                if (proc.GetType().GetInterfaces().Contains(typeof(IThiqahInboxProcessor)))
                {
                    await ((IThiqahInboxProcessor)proc).Process();
                }
            }
        }

        public async Task RunOutboxSenders()
        {
            var procs = Senders;
            CLogger.Log($"RUNNING SENDERS {procs.Count}");
            foreach (var proc in procs)
            {
                if (proc.GetType().GetInterfaces().Contains(typeof(IThiqahOutboxSender)))
                {
                    CLogger.Log($"INVOKE: {proc.GetType().Name}");
                    await ((IThiqahOutboxSender)proc).SendEvents();
                }
            }
        }
    }
}
