using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Threading;
using Codeshell.Abp.DistributedEventBoxes.RabbitMq;
using Volo.Abp.RabbitMQ;

namespace Codeshell.Abp.DistributedEventBoxes.RabbitMq
{
    [ExposeServices(typeof(RabbitMqMessageConsumer))]
    public class CodeshellRabbitMqMessageConsumer : RabbitMqMessageConsumer, ICodeshellRabbitMqMessageConsumer
    {
        public CodeshellRabbitMqMessageConsumer(IConnectionPool connectionPool, AbpAsyncTimer timer, IExceptionNotifier exceptionNotifier) : base(connectionPool, timer, exceptionNotifier)
        {

        }

        public override Task BindAsync(string routingKey)
        {

            return base.BindAsync(routingKey);
        }

        public void InitializeQueue(bool quorum = true)
        {
            if (quorum)
                Queue.Arguments.Add("x-queue-type", "quorum");
            Callbacks.Clear();
        }
    }
}
