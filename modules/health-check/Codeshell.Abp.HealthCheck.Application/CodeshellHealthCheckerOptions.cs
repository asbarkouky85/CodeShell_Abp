using System;
using System.Collections.Generic;
using Codeshell.Abp.HealthCheck.Application;

namespace Codeshell.Abp.HealthCheck.Application
{
    public class CodeshellHealthCheckerOptions
    {
        public bool Enabled { get; set; }
        public bool LogSuccess { get; set; }
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);
        public List<CheckServiceItem> Services { get; set; }
    }
}
