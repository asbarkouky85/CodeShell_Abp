using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Maneh.RabbitMQTest
{
    public class ConsoleTestAppHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = AbpApplicationFactory.Create<RabbitMQConsole2Module>(opts =>
            {
                opts.UseAutofac();
            }))
            {

                application.Initialize();

                var demo = application.ServiceProvider.GetRequiredService<Console2Service>();

                await demo.RunAsync();

                application.Shutdown();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
