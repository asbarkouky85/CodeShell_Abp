using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Codeshell.Abp.DistributedEventBoxes.Inbox
{
    public interface IEventBoxesJobAccessor : ITransientDependency
    {
        List<IInboxProcessor> Processors { get; }
        List<IOutboxSender> Senders { get; }
        Task RunInboxProcessors();
        Task RunOutboxSenders();
    }
}
