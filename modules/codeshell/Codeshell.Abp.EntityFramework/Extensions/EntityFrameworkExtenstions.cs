using Codeshell.Abp.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Codeshell.Abp.Extensions.EntityFramework
{
    public static class EntityFrameworkExtenstions
    {
        public static void UpdateDatabaseStructure<TDbContext>(this IServiceProvider prov) where TDbContext : DbContext
        {
            AsyncHelper.RunSync(async () =>
            {
                using var scope = prov.CreateScope();
                var s = scope.ServiceProvider
                    .GetRequiredService<TDbContext>();

                await s.Database.MigrateAsync();
            });
        }

        public static string ScriptPendingMigrations(this DbContext context)
        {
            var fromMigration = context.Database.GetAppliedMigrations().LastOrDefault();

            var script = context.GetService<IMigrator>().GenerateScript(fromMigration);

            return script;
        }

        public static string GetLastAppliedMigration(this DbContext context)
        {
            var fromMigration = context.Database.GetAppliedMigrations().LastOrDefault();

            return fromMigration ?? "";
        }

        public static void ConfigureTwoLanguage<T>(this EntityTypeBuilder<T> entity) where T : class, ITwoLanguage
        {
            entity.Property(e => e.NameAr).IsUnicode().HasMaxLength(255);
            entity.Property(e => e.NameEn).IsUnicode(false).HasMaxLength(255);
        }

        public static async Task RollbackMigrationsAsync(this DbContext context, string targetMigration)
        {
            Check.NotNullOrEmpty(targetMigration, nameof(targetMigration));

            await context.GetService<IMigrator>().MigrateAsync(targetMigration);

        }
    }
}
