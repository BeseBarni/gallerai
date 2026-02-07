using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallerai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageEntityConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Images",
                newName: "R2Key");

            migrationBuilder.RenameIndex(
                name: "IX_Images_Key",
                table: "Images",
                newName: "IX_Images_R2Key");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Images",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "R2Key",
                table: "Images",
                newName: "Key");

            migrationBuilder.RenameIndex(
                name: "IX_Images_R2Key",
                table: "Images",
                newName: "IX_Images_Key");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Images",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);
        }
    }
}
