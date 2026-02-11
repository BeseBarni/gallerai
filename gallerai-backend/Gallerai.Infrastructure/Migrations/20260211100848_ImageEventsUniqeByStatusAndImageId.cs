using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallerai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageEventsUniqeByStatusAndImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ImageEvents_ImageId_Status_Unique",
                table: "ImageEvents",
                columns: new[] { "ImageId", "Status" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageEvents_ImageId_Status_Unique",
                table: "ImageEvents");
        }
    }
}
