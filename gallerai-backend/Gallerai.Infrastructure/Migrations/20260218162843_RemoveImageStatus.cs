using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallerai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageStates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageStates",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageStates", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ImageStates_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageStates_Status",
                table: "ImageStates",
                column: "Status");
        }
    }
}
