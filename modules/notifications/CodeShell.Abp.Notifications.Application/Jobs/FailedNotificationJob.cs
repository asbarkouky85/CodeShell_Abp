using Codeshell.Abp.Notifications.Senders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Jobs
{
    public class FailedNotificationJob
    {
        public bool RunOnStartUp => true;
        public TimeSpan Interval => new TimeSpan(2, 0, 0);

        public async Task Run(IServiceProvider provider)
        {
            await provider.GetService<INotificationDeliveryService>().RetryFailedMessages();
        }
    }
}
