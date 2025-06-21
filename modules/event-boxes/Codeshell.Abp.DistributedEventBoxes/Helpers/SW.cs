using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Codeshell.Abp.DistributedEventBoxes.Helpers;

namespace Codeshell.Abp.DistributedEventBoxes.Helpers
{
    public class SW
    {
        private SW() { }

        public static TimeConsumption Measure(string logName = null)
        {
            return new TimeConsumption(logName);
        }
    }
}

