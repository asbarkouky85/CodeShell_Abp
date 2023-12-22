using Microsoft.EntityFrameworkCore;
using Codeshell.Abp.Attachments.Attachments;
using Codeshell.Abp.Attachments.Permissions;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    public static class AttachmentsDbContextModelCreatingExtensions
    {
        public static void ConfigureAttachments(
            this ModelBuilder modelBuilder,
            Action<AttachmentsModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var options = new AttachmentsModelBuilderConfigurationOptions(
                AttachmentsDbProperties.DbTablePrefix,
                AttachmentsDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            modelBuilder.Entity<Attachment>(entity =>
            {

                entity.ToTable(options.TablePrefix + "Attachments");
                entity.ConfigureByConvention();
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.FullPath).HasMaxLength(300);
                entity.Property(e => e.ContainerName).HasMaxLength(60);

                entity.Property(e => e.Extension).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.ContentType).HasMaxLength(255).IsUnicode(false);


                entity.HasOne(d => d.AttachmentCategory)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.AttachmentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachments_AttachmentCategories");

                entity.HasOne(d => d.BinaryAttachment)
                    .WithOne(p => p.Attachment)
                    .HasForeignKey<Attachment>(d => d.BinaryAttachmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Attachments_BinaryAttachments");
            });

            modelBuilder.Entity<AttachmentCategory>(entity =>
            {
                entity.ToTable(options.TablePrefix + "AttachmentCategories");
                entity.ConfigureByConvention();
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FolderPath).HasMaxLength(150);
                entity.Property(e => e.ContainerName).HasMaxLength(60);
                entity.OwnsOne(e => e.MaxDimension, e =>
                {
                    e.Property(d => d.Height).HasColumnName("MaxDimensionHeight");
                    e.Property(d => d.Width).HasColumnName("MaxDimensionWidth");
                });

                entity.Property(e => e.NameEn)
                  .HasMaxLength(50)
                  .IsUnicode(false);
                entity.Property(e => e.NameAr)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ValidExtensions).HasMaxLength(500).IsUnicode(false);
            });

            modelBuilder.Entity<AttachmentCategoryPermission>(entity =>
            {
                entity.ToTable(options.TablePrefix + "AttachmentCategoryPermissions");
                entity.Property(e => e.Role).HasMaxLength(100);
                entity.HasOne(d => d.AttachmentCategory)
                .WithMany(d => d.AttachmentCategoryPermissions)
                .HasForeignKey(d => d.AttachmentCategoryId);
            });

            modelBuilder.Entity<AttachmentSetting>(entity =>
            {
                entity.ToTable(options.TablePrefix + "AttachmentSettings");
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AttachmentBinary>(entity =>
            {
                entity.ToTable(options.TablePrefix + "AttachmentBinaries");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Bytes).HasMaxLength(4000);
            });

            modelBuilder.Entity<TempFile>(entity =>
            {
                entity.ToTable(options.TablePrefix + "TempFiles");
                entity.ConfigureByConvention();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.ContentType).HasMaxLength(60).IsUnicode(false);
                entity.Property(e => e.Extension).HasMaxLength(6).IsUnicode(false);
            });
        }
    }
}