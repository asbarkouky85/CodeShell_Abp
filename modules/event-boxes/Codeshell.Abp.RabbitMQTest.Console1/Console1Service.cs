using Maneh.RabbitMQTest.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Maneh.RabbitMQTest
{
    public class Console1Service : ITransientDependency
    {
        private readonly IDistributedEventBus bus;

        public Console1Service(IDistributedEventBus bus)
        {
            this.bus = bus;
        }
        public Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("Service 1 is Running...");
                Console.Write("Press Enter key to send event..");
                Console.ReadLine();
                _ = bus.PublishAsync(new MessageAdded("Message Id is {0}"));

            }

        }
    }
}
