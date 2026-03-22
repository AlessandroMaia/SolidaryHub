using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Campaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "campaign_db");

            migrationBuilder.CreateTable(
                name: "campaigns",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    financial_goal_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    financial_goal_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "donation_intents",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<int>(type: "integer", nullable: false),
                    donor_user_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    message_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    requested_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    validated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_intents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaign_status_history",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<int>(type: "integer", nullable: false),
                    old_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    new_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    changed_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_campaign_status_history_campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalSchema: "campaign_db",
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTotals",
                schema: "campaign_db",
                columns: table => new
                {
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    total_amount_raised = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_donations_count = table.Column<int>(type: "integer", nullable: false),
                    last_donation_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "donation_ledger_entries",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<int>(type: "integer", nullable: false),
                    donation_intent_id = table.Column<int>(type: "integer", nullable: false),
                    donor_user_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_ledger_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_donation_ledger_entries_campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalSchema: "campaign_db",
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donation_intent_dead_letters",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    donation_intent_id = table.Column<int>(type: "integer", nullable: false),
                    message_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    campaign_id = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: false),
                    failed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_intent_dead_letters", x => x.id);
                    table.ForeignKey(
                        name: "FK_donation_intent_dead_letters_donation_intents_donation_inte~",
                        column: x => x.donation_intent_id,
                        principalSchema: "campaign_db",
                        principalTable: "donation_intents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donation_intent_processing_logs",
                schema: "campaign_db",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    donation_intent_id = table.Column<int>(type: "integer", nullable: false),
                    campaign_id = table.Column<int>(type: "integer", nullable: false),
                    worker_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_attempt_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_intent_processing_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_donation_intent_processing_logs_donation_intents_donation_i~",
                        column: x => x.donation_intent_id,
                        principalSchema: "campaign_db",
                        principalTable: "donation_intents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_status_history_campaign_id",
                schema: "campaign_db",
                table: "campaign_status_history",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_intent_dead_letters_donation_intent_id",
                schema: "campaign_db",
                table: "donation_intent_dead_letters",
                column: "donation_intent_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_intent_processing_logs_donation_intent_id",
                schema: "campaign_db",
                table: "donation_intent_processing_logs",
                column: "donation_intent_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_intents_message_id",
                schema: "campaign_db",
                table: "donation_intents",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_ledger_entries_campaign_id",
                schema: "campaign_db",
                table: "donation_ledger_entries",
                column: "campaign_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_status_history",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "CampaignTotals",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "donation_intent_dead_letters",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "donation_intent_processing_logs",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "donation_ledger_entries",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "donation_intents",
                schema: "campaign_db");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "campaign_db");
        }
    }
}
