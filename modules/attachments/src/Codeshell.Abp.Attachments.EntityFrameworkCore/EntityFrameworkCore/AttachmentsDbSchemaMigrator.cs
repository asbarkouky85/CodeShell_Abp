using Codeshell.Abp.Extensions.EntityFramework;
using DataBoat.ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    public class AttachmentsDbSchemaMigrator : IAttachmentsDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public AttachmentsDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string GetLastAppliedMigration()
        {
            var context = _serviceProvider
              .GetRequiredService<AttachmentsDbContext>();

            return context.GetLastAppliedMigration();
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the SystemMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<AttachmentsDbContext>()
                .Database
                .MigrateAsync();
        }

        public async Task RollbackMigrationsAsync(string targetMigration)
        {
            var context = _serviceProvider
               .GetRequiredService<AttachmentsDbContext>();

            await context.RollbackMigrationsAsync(targetMigration);
        }

        public string ScriptMigrations()
        {
            var context = _serviceProvider
                .GetRequiredService<AttachmentsDbContext>();

            return context.ScriptPendingMigrations();

        }
    }
}