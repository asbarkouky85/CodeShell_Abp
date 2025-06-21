using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.HealthCheck
{
    public interface IHealthCheckCleanupService
    {
        Task CleanUp();
    }


}
