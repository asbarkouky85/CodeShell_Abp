using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.HealthCheck
{
    public class HealthCheckCleanupService : DomainService, IHealthCheckCleanupService
    {
        private readonly HealthCheckDbContext context;

        public HealthCheckCleanupService(HealthCheckDbContext context)
        {
            this.context = context;
        }

        public async Task CleanUp()
        {
            try
            {
                var date = DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");
                await context.Database.ExecuteSqlRawAsync($"delete from CheckItems where CreatedOn<'{date}' AND StatusCode=200");

            }
            catch
            {

            }
        }
    }
}
