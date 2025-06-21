using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.HealthCheck
{
    public static class HealthCheckEntityFrameworkExtensions
    {
        public static void AddHealthCheckEntityFramework(this IServiceCollection coll, bool asDefaultModule, string migrationAssembly = null)
        {

        }
    }
}
