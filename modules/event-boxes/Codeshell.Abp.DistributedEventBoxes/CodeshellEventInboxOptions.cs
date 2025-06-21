using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeshell.Abp.DistributedEventBoxes.DistributedLock;

namespace Codeshell.Abp.DistributedEventBoxes
{
    public class CodeshellEventInboxOptions
    {
        public int ParallelThreadCount { get; set; } = 3;
        public bool ShowConsoleLogs { get; set; } = false;
        public bool ParallelHandling { get; set; } = true;
        public DistributedLockTypes LockType { get; set; } = DistributedLockTypes.Redis;
        public bool Enable { get; set; } = true;
        /// <summary>
        /// Default = 3
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        /// <summary>
        /// Default = 10
        /// </summary>
        public int RetryIntervalInSeconds { get; set; } = 10;
        /// <summary>
        /// Default = 240
        /// </summary>
        public int RetryRecycleTimeInMinutes { get; set; } = 240;
        public TimeSpan PeriodTimeSpan { get; set; } = TimeSpan.FromMinutes(1);
        public string[] SequentialHandlingEvents { get; set; }
    }
}
