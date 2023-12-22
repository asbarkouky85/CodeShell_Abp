using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DataBoat.ECommerce.Data;

/* This is used if database provider does't define
 * IECommerceDbSchemaMigrator implementation.
 */
public class NullAttachmentsDbSchemaMigrator : IAttachmentsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
