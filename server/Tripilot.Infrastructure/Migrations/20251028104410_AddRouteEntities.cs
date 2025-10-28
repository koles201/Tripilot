using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Privacy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EstimatedDuration = table.Column<int>(type: "integer", nullable: false),
                    TotalDistance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FavoriteCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutePlaces",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EstimatedTimeAtPlace = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePlaces", x => new { x.RouteId, x.PlaceId });
                    table.ForeignKey(
                        name: "FK_RoutePlaces_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutePlaces_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePlaces_PlaceId",
                table: "RoutePlaces",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePlaces_RouteId",
                table: "RoutePlaces",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePlaces_RouteId_Order",
                table: "RoutePlaces",
                columns: new[] { "RouteId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_AverageRating",
                table: "Routes",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_CreatorId",
                table: "Routes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Difficulty",
                table: "Routes",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_IsActive",
                table: "Routes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_IsFeatured",
                table: "Routes",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Name",
                table: "Routes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Privacy",
                table: "Routes",
                column: "Privacy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutePlaces");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
