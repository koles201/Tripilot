using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Location_Latitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: false),
                    Location_Longitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: false),
                    Location_Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Location_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContactInfo_Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactInfo_Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContactInfo_Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OperatingHours_OpenTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    OperatingHours_CloseTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    OperatingHours_DaysOfWeek = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OperatingHours_Is24Hours = table.Column<bool>(type: "boolean", nullable: true),
                    OperatingHours_SpecialNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PriceLevel = table.Column<int>(type: "integer", nullable: true),
                    Amenities = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GalleryImages = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Places_AverageRating",
                table: "Places",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Category",
                table: "Places",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Places_IsActive",
                table: "Places",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Name",
                table: "Places",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Places_OwnerId",
                table: "Places",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
