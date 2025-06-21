using Maneh.RabbitMQTest.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Maneh.RabbitMQTest.Handlers
{
    public class MessageHandler : ITransientDependency, IDistributedEventHandler<MessageAdded>
    {
        public Task HandleEventAsync(MessageAdded eventData)
        {
            return Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Event Consumed : " + eventData);
                Console.ForegroundColor = ConsoleColor.Gray;
            });
        }
    }
}
