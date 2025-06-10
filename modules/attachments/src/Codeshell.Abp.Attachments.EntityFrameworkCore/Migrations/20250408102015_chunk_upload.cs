using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeshell.Abp.Attachments.Migrations
{
    public partial class chunk_upload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Att_TempFiles;");
            migrationBuilder.AddColumn<int>(
                name: "AttachmentCategoryId",
                table: "Att_TempFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Att_TempFiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Att_TempFiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalChunkCount",
                table: "Att_TempFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Att_Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Att_AttachmentCategories",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Att_AttachmentCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Att_TempFileChunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TempFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChunkIndex = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Att_TempFileChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Att_TempFileChunks_Att_TempFiles_TempFileId",
                        column: x => x.TempFileId,
                        principalTable: "Att_TempFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Att_TempFiles_AttachmentCategoryId",
                table: "Att_TempFiles",
                column: "AttachmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Att_TempFileChunks_TempFileId",
                table: "Att_TempFileChunks",
                column: "TempFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Att_TempFiles_Att_AttachmentCategories_AttachmentCategoryId",
                table: "Att_TempFiles",
                column: "AttachmentCategoryId",
                principalTable: "Att_AttachmentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Att_TempFiles_Att_AttachmentCategories_AttachmentCategoryId",
                table: "Att_TempFiles");

            migrationBuilder.DropTable(
                name: "Att_TempFileChunks");

            migrationBuilder.DropIndex(
                name: "IX_Att_TempFiles_AttachmentCategoryId",
                table: "Att_TempFiles");

            migrationBuilder.DropColumn(
                name: "AttachmentCategoryId",
                table: "Att_TempFiles");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Att_TempFiles");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Att_TempFiles");

            migrationBuilder.DropColumn(
                name: "TotalChunkCount",
                table: "Att_TempFiles");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Att_Attachments");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Att_AttachmentCategories",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Att_AttachmentCategories",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
