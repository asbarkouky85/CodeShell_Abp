﻿// <auto-generated />
using System;
using Codeshell.Abp.Attachments.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace Codeshell.Abp.Attachments.Migrations
{
    [DbContext(typeof(AttachmentsDbContext))]
    [Migration("20250211181930_content_type_length")]
    partial class content_type_length
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.SqlServer)
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Codeshell.Abp.Attachments.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AttachmentCategoryId")
                        .HasColumnType("int");

                    b.Property<Guid?>("BinaryAttachmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContainerName")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("ContentType")
                        .HasMaxLength(800)
                        .IsUnicode(false)
                        .HasColumnType("varchar(800)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("DeleterId");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("DeletionTime");

                    b.Property<string>("Extension")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("FileName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FullPath")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentCategoryId");

                    b.HasIndex("BinaryAttachmentId")
                        .IsUnique()
                        .HasFilter("[BinaryAttachmentId] IS NOT NULL");

                    b.ToTable("Att_Attachments", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentBinary", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Bytes")
                        .HasMaxLength(4000)
                        .HasColumnType("varbinary(4000)");

                    b.HasKey("Id");

                    b.ToTable("Att_AttachmentBinaries", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentCategory", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("AnonymousDownload")
                        .HasColumnType("bit");

                    b.Property<string>("ContainerName")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<string>("FolderPath")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.Property<int?>("MaxCount")
                        .HasColumnType("int");

                    b.Property<int>("MaxSize")
                        .HasColumnType("int");

                    b.Property<string>("NameAr")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("NameEn")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ValidExtensions")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Att_AttachmentCategories", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.Property<string>("Name")
                        .HasMaxLength(60)
                        .IsUnicode(false)
                        .HasColumnType("varchar(60)");

                    b.Property<string>("Value")
                        .HasMaxLength(300)
                        .IsUnicode(false)
                        .HasColumnType("varchar(300)");

                    b.HasKey("Id");

                    b.ToTable("Att_AttachmentSettings", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.Permissions.AttachmentCategoryPermission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int>("AttachmentCategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<bool>("Download")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.Property<string>("Role")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("Upload")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentCategoryId");

                    b.ToTable("Att_AttachmentCategoryPermissions", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.TempFile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContentType")
                        .HasMaxLength(800)
                        .IsUnicode(false)
                        .HasColumnType("varchar(800)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<string>("Extension")
                        .HasMaxLength(6)
                        .IsUnicode(false)
                        .HasColumnType("varchar(6)");

                    b.Property<string>("FileName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FullPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.HasKey("Id");

                    b.ToTable("Att_TempFiles", (string)null);
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.Attachment", b =>
                {
                    b.HasOne("Codeshell.Abp.Attachments.AttachmentCategory", "AttachmentCategory")
                        .WithMany("Attachments")
                        .HasForeignKey("AttachmentCategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Attachments_AttachmentCategories");

                    b.HasOne("Codeshell.Abp.Attachments.AttachmentBinary", "BinaryAttachment")
                        .WithOne("Attachment")
                        .HasForeignKey("Codeshell.Abp.Attachments.Attachment", "BinaryAttachmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Attachments_BinaryAttachments");

                    b.Navigation("AttachmentCategory");

                    b.Navigation("BinaryAttachment");
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentCategory", b =>
                {
                    b.OwnsOne("Codeshell.Abp.Attachments.Dimension", "MaxDimension", b1 =>
                        {
                            b1.Property<int>("AttachmentCategoryId")
                                .HasColumnType("int");

                            b1.Property<int>("Height")
                                .HasColumnType("int")
                                .HasColumnName("MaxDimensionHeight");

                            b1.Property<int>("Width")
                                .HasColumnType("int")
                                .HasColumnName("MaxDimensionWidth");

                            b1.HasKey("AttachmentCategoryId");

                            b1.ToTable("Att_AttachmentCategories");

                            b1.WithOwner()
                                .HasForeignKey("AttachmentCategoryId");
                        });

                    b.Navigation("MaxDimension");
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.Permissions.AttachmentCategoryPermission", b =>
                {
                    b.HasOne("Codeshell.Abp.Attachments.AttachmentCategory", "AttachmentCategory")
                        .WithMany("AttachmentCategoryPermissions")
                        .HasForeignKey("AttachmentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AttachmentCategory");
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentBinary", b =>
                {
                    b.Navigation("Attachment");
                });

            modelBuilder.Entity("Codeshell.Abp.Attachments.AttachmentCategory", b =>
                {
                    b.Navigation("AttachmentCategoryPermissions");

                    b.Navigation("Attachments");
                });
#pragma warning restore 612, 618
        }
    }
}
