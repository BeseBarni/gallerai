using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallerai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class R2UploadNotifChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Images");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Images",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Images",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
