using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    [ConnectionStringName(AttachmentsDbProperties.ConnectionStringName)]
    public class AttachmentsDbContext : AbpDbContext<AttachmentsDbContext>, IAttachmentsDbContext
    {
        public virtual DbSet<Attachment> Attachments { get; set; }
        public DbSet<AttachmentCategory> AttachmentCategories { get; set; }
        public virtual DbSet<AttachmentSetting> AttachmentSettings { get; set; }
        public virtual DbSet<AttachmentBinary> BinaryAttachments { get; set; }
        public virtual DbSet<TempFile> TempFiles { get; set; }

        public AttachmentsDbContext(DbContextOptions<AttachmentsDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureAttachments();
        }
    }
}