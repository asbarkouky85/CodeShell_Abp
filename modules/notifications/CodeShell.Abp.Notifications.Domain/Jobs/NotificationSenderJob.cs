using Codeshell.Abp.Notifications.Senders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications.Jobs
{
    public class NotificationSenderJob
    {
        public bool RunOnStartUp => true;

        public TimeSpan Interval => new TimeSpan(1, 0, 0);

        public async Task Run(IServiceProvider provider)
        {
            var service = provider.GetRequiredService<INotificationDeliveryService>();
            await service.SendPendingMessages();
        }
    }
}
