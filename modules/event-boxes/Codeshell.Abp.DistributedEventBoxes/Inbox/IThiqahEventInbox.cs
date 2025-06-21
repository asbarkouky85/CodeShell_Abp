using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.DistributedEventBoxes.Inbox
{
    public interface IThiqahEventInbox : ITransientDependency
    {
        Task UpdateRetries(Guid id, int retries, string exception);

    }
}
