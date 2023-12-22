using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeshell.Abp.Attachments.Migrations
{
    public partial class Att_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Att_AttachmentBinaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bytes = table.Column<byte[]>(type: "varbinary(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_AttachmentBinaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Att_AttachmentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameEn = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NameAr = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ValidExtensions = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    MaxSize = table.Column<int>(type: "int", nullable: false),
                    FolderPath = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MaxDimensionWidth = table.Column<int>(type: "int", nullable: true),
                    MaxDimensionHeight = table.Column<int>(type: "int", nullable: true),
                    AnonymousDownload = table.Column<bool>(type: "bit", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_AttachmentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Att_AttachmentSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: true),
                    Value = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_AttachmentSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Att_TempFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: true),
                    ContentType = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_TempFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Att_AttachmentCategoryPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttachmentCategoryId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Upload = table.Column<bool>(type: "bit", nullable: false),
                    Download = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_AttachmentCategoryPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Att_AttachmentCategoryPermissions_Att_AttachmentCategories_AttachmentCategoryId",
                        column: x => x.AttachmentCategoryId,
                        principalTable: "Att_AttachmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Att_Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AttachmentCategoryId = table.Column<int>(type: "int", nullable: false),
                    BinaryAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Extension = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    ContentType = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_AttachmentCategories",
                        column: x => x.AttachmentCategoryId,
                        principalTable: "Att_AttachmentCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachments_BinaryAttachments",
                        column: x => x.BinaryAttachmentId,
                        principalTable: "Att_AttachmentBinaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Att_AttachmentCategoryPermissions_AttachmentCategoryId",
                table: "Att_AttachmentCategoryPermissions",
                column: "AttachmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Att_Attachments_AttachmentCategoryId",
                table: "Att_Attachments",
                column: "AttachmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Att_Attachments_BinaryAttachmentId",
                table: "Att_Attachments",
                column: "BinaryAttachmentId",
                unique: true,
                filter: "[BinaryAttachmentId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Att_AttachmentCategoryPermissions");

            migrationBuilder.DropTable(
                name: "Att_Attachments");

            migrationBuilder.DropTable(
                name: "Att_AttachmentSettings");

            migrationBuilder.DropTable(
                name: "Att_TempFiles");

            migrationBuilder.DropTable(
                name: "Att_AttachmentCategories");

            migrationBuilder.DropTable(
                name: "Att_AttachmentBinaries");
        }
    }
}
