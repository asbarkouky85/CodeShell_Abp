using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Codeshell.Abp.Attachments.Attachments;

namespace Codeshell.Abp.Attachments.Jobs
{
    public class TempFileCleanUpJob : AsyncPeriodicBackgroundWorkerBase
    {
        //static int _period = 30 * 1000;
        static int _period = 12 * 60 * 60 * 1000;
        static TimeSpan _keepFilesFor = new TimeSpan(2, 0, 0);

        public TempFileCleanUpJob(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            timer.Period = _period;
            timer.RunOnStart = true;
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            using (var sc = ServiceScopeFactory.CreateScope())
            {
                var temp = sc.ServiceProvider.GetRequiredService<ITempFileService>();
                var date = DateTime.Now - _keepFilesFor;
                await temp.CleanUp(date);
            }
        }
    }
}
