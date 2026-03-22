using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Campaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCampaignTotalValueObjectMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignTotals",
                schema: "campaign_db");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_donation_at",
                schema: "campaign_db",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount_raised",
                schema: "campaign_db",
                table: "campaigns",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "total_donations_count",
                schema: "campaign_db",
                table: "campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "total_updated_at",
                schema: "campaign_db",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_donation_at",
                schema: "campaign_db",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "total_amount_raised",
                schema: "campaign_db",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "total_donations_count",
                schema: "campaign_db",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "total_updated_at",
                schema: "campaign_db",
                table: "campaigns");

            migrationBuilder.CreateTable(
                name: "CampaignTotals",
                schema: "campaign_db",
                columns: table => new
                {
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    last_donation_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_amount_raised = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_donations_count = table.Column<int>(type: "integer", nullable: false),
                    total_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTotals", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_CampaignTotals_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "campaign_db",
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
