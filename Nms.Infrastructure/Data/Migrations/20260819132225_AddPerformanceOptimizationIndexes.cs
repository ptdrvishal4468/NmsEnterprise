using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceOptimizationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "DeviceId",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "Facility",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "Severity",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "SourceIpAddress",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "TimestampUtc",
                table: "SyslogMessages");

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_DeviceId",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_DeviceId_TimestampUtc",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "DeviceId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_Facility",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "Facility" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_Severity",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_Severity_TimestampUtc",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "Severity", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_SourceIpAddress",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "SourceIpAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId_TimestampUtc",
                table: "SyslogMessages",
                columns: new[] { "TenantId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId_CreatedAtUtc",
                table: "Devices",
                columns: new[] { "TenantId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId_Status_DeviceType_CreatedAtUtc",
                table: "Devices",
                columns: new[] { "TenantId", "Status", "DeviceType", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMetricsRaw_TenantId_TimestampUtc",
                table: "DeviceMetricsRaw",
                columns: new[] { "TenantId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_UserId_TimestampUtc",
                table: "AuditLogs",
                columns: new[] { "TenantId", "UserId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TenantId_State_Severity_TriggeredAtUtc",
                table: "Alerts",
                columns: new[] { "TenantId", "State", "Severity", "TriggeredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_DeviceId",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_DeviceId_TimestampUtc",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_Facility",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_Severity",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_Severity_TimestampUtc",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_SourceIpAddress",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_SyslogMessages_TenantId_TimestampUtc",
                table: "SyslogMessages");

            migrationBuilder.DropIndex(
                name: "IX_Devices_TenantId_CreatedAtUtc",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_TenantId_Status_DeviceType_CreatedAtUtc",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_DeviceMetricsRaw_TenantId_TimestampUtc",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TenantId_UserId_TimestampUtc",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_TenantId_State_Severity_TriggeredAtUtc",
                table: "Alerts");

            migrationBuilder.CreateIndex(
                name: "DeviceId",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "Facility",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "Severity",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "SourceIpAddress",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "TimestampUtc",
                table: "SyslogMessages",
                column: "TenantId");
        }
    }
}
