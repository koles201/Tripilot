using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceClaimEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimantUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClaimReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DocumentUrls = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminDecisionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceClaims_BusinessProfiles_BusinessProfileId",
                        column: x => x.BusinessProfileId,
                        principalTable: "BusinessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceClaims_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceClaims_Users_ClaimantUserId",
                        column: x => x.ClaimantUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceClaims_Users_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_BusinessProfileId",
                table: "PlaceClaims",
                column: "BusinessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_ClaimantUserId",
                table: "PlaceClaims",
                column: "ClaimantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_PlaceId",
                table: "PlaceClaims",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_PlaceId_Status",
                table: "PlaceClaims",
                columns: new[] { "PlaceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_ReviewedByAdminId",
                table: "PlaceClaims",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceClaims_Status",
                table: "PlaceClaims",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceClaims");
        }
    }
}
