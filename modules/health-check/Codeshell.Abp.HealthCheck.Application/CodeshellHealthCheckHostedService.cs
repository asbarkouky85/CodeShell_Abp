using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Http;
using Codeshell.Abp.Helpers;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.HealthCheck.Application
{
    public class CodeshellHealthCheckHostedService : BackgroundService, IHostedService, IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected Timer Timer;
        protected CodeshellHealthCheckerOptions Options { get; private set; }
        private volatile bool _isActive = true;
        DateTime _lastCleanupDay = DateTime.MinValue;

        public CodeshellHealthCheckHostedService(IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            this.ServiceProvider = serviceProvider;
            Options = ServiceProvider.GetService<IOptions<CodeshellHealthCheckerOptions>>().Value;
        }

        void Run(object state)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var serviceItem in Options.Services)
                {
                    var t = CheckService(serviceItem);
                    t.ConfigureAwait(false);
                    tasks.Add(t);
                }
                _ = Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Run Cycle Failed");
            }

        }

        public async Task CheckService(CheckServiceItem item)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var watch = SW.Measure();
                var cleaner = scope.ServiceProvider.GetRequiredService<IHealthCheckCleanupService>();
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<CheckItem, long>>();
                var todayStart = DateTime.Now.GetDayStart();
                if (todayStart > _lastCleanupDay)
                {
                    _lastCleanupDay = todayStart;
                    await cleaner.CleanUp();
                }

                try
                {
                    var service = new HttpService(item.Host);

                    var res = await service.GetResponseAsync(item.Url ?? "health");
                    var response = await res.Content.ReadAsStringAsync();

                    var entry = new CheckItem(item.Name, item.Host, watch.Elapsed.TotalMilliseconds);
                    if (res.IsSuccessStatusCode)
                    {
                        entry.SetSuccess((int)res.StatusCode, response);
                        if (Options.LogSuccess)
                            await repo.InsertAsync(entry);
                    }
                    else
                    {
                        entry.SetFailed((int)res.StatusCode, response);
                        await repo.InsertAsync(entry);
                    }

                }
                catch (Exception ex)
                {
                    var entry = new CheckItem(item.Name, item.Host, watch.Elapsed.TotalMilliseconds);
                    entry.SetFailed(0, ex.GetMessageRecursive());
                    await repo.InsertAsync(entry);
                }

            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Timer.Stop();
                _isActive = false;
            });

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Timer = new Timer();
                Timer.Interval = Options.Period.TotalMilliseconds;
                Timer.Elapsed += (e, a) => Run(e);
                Timer.Enabled = true;
                Timer.AutoReset = true;
                Timer.Start();
                // while (_isActive) ;
            });
        }
    }
}
