using Codeshell.Abp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    [DependsOn(
        typeof(AttachmentsDomainModule),
        typeof(CodeshellEntityFrameworkCoreModule)
    )]
    public class AttachmentsEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AttachmentsDbContext>(options =>
            {
                options.AddDefaultRepositories(true);
                /** Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
            });
            context.Services.AddTransient<ICodeshellDbSchemaMigrator, AttachmentsDbSchemaMigrator>();
            Configure<AbpDbContextOptions>(e =>
            {
                e.UseSqlServer();
            });

            Configure<AbpEntityOptions>(options =>
            {
                options.Entity<AttachmentCategory>(e =>
                {
                    e.DefaultWithDetailsFunc = q => q.Include(e => e.AttachmentCategoryPermissions);
                });
            });
        }
    }
}