using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallerai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                });

            migrationBuilder.CreateTable(
                name: "ImageTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageAnalyses",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    AestheticScore = table.Column<double>(type: "double precision", nullable: false),
                    Critique = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnalyses", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ImageAnalyses_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageEvents",
                columns: table => new
                {
                    ImageEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageEvents", x => x.ImageEventId);
                    table.ForeignKey(
                        name: "FK_ImageEvents_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageMetadata",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Camera_Make = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Camera_Model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Camera_LensModel = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Camera_Software = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Camera_CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Exposure_Iso = table.Column<int>(type: "integer", nullable: true),
                    Exposure_Aperture = table.Column<double>(type: "double precision", nullable: true),
                    Exposure_ShutterSpeedSeconds = table.Column<double>(type: "double precision", nullable: true),
                    Exposure_FocalLengthMm = table.Column<double>(type: "double precision", nullable: true),
                    Exposure_ExposureCompensation = table.Column<double>(type: "double precision", nullable: true),
                    Exposure_Flash = table.Column<int>(type: "integer", nullable: true),
                    Exposure_WhiteBalance = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageMetadata", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ImageMetadata_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ImageImageTag",
                columns: table => new
                {
                    ImageListImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageTagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageImageTag", x => new { x.ImageListImageId, x.ImageTagsId });
                    table.ForeignKey(
                        name: "FK_ImageImageTag_ImageTags_ImageTagsId",
                        column: x => x.ImageTagsId,
                        principalTable: "ImageTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageImageTag_Images_ImageListImageId",
                        column: x => x.ImageListImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageEvents_ImageId_LastUpdate",
                table: "ImageEvents",
                columns: new[] { "ImageId", "LastUpdate" });

            migrationBuilder.CreateIndex(
                name: "IX_ImageImageTag_ImageTagsId",
                table: "ImageImageTag",
                column: "ImageTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Key",
                table: "Images",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadedAt",
                table: "Images",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ImageStates_Status",
                table: "ImageStates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_Tag",
                table: "ImageTags",
                column: "Tag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageAnalyses");

            migrationBuilder.DropTable(
                name: "ImageEvents");

            migrationBuilder.DropTable(
                name: "ImageImageTag");

            migrationBuilder.DropTable(
                name: "ImageMetadata");

            migrationBuilder.DropTable(
                name: "ImageStates");

            migrationBuilder.DropTable(
                name: "ImageTags");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
