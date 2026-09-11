using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNmsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ThreatDetectionRules_TenantId_IsEnabled_RuleName",
                table: "ThreatDetectionRules",
                columns: new[] { "TenantId", "IsEnabled", "RuleName" });

            migrationBuilder.CreateIndex(
                name: "IX_PollProfiles_TenantId_IsDefault_IsEnabled",
                table: "PollProfiles",
                columns: new[] { "TenantId", "IsDefault", "IsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThreatDetectionRules_TenantId_IsEnabled_RuleName",
                table: "ThreatDetectionRules");

            migrationBuilder.DropIndex(
                name: "IX_PollProfiles_TenantId_IsDefault_IsEnabled",
                table: "PollProfiles");
        }
    }
}
