using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    [ConnectionStringName(AttachmentsDbProperties.ConnectionStringName)]
    public interface IAttachmentsDbContext : IEfCoreDbContext
    {
        /** Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}