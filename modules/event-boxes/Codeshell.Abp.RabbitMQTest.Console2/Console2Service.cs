using Maneh.RabbitMQTest.Events;
using Maneh.RabbitMQTest.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Maneh.RabbitMQTest
{
    public class Console2Service : ITransientDependency
    {
        private readonly IDistributedEventBus bus;

        public Console2Service(IDistributedEventBus bus)
        {
            this.bus = bus;
            
        }
        public  Task RunAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Service 2 is Running...");
                while (true) ;
            });
            
        }
    }
}
