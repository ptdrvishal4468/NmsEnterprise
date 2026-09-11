using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatDetectionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationDriftRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaselineBackupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentBackupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasDrift = table.Column<bool>(type: "bit", nullable: false),
                    AddedLinesCount = table.Column<int>(type: "int", nullable: false),
                    RemovedLinesCount = table.Column<int>(type: "int", nullable: false),
                    ModifiedLinesCount = table.Column<int>(type: "int", nullable: false),
                    DifferencesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgmentNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDriftRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDriftRecords_ConfigurationBackups_BaselineBackupId",
                        column: x => x.BaselineBackupId,
                        principalTable: "ConfigurationBackups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationDriftRecords_ConfigurationBackups_CurrentBackupId",
                        column: x => x.CurrentBackupId,
                        principalTable: "ConfigurationBackups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationDriftRecords_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThreatDetectionRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ThreatType = table.Column<int>(type: "int", nullable: false),
                    DefaultSeverity = table.Column<int>(type: "int", nullable: false),
                    FailureThreshold = table.Column<int>(type: "int", nullable: false),
                    TimeWindowMinutes = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatDetectionRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreatIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThreatType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SourceIp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetUser = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    FirstDetectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDetectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IndicatorMetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreatIndicators_Devices_TargetDeviceId",
                        column: x => x.TargetDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDriftRecords_BaselineBackupId",
                table: "ConfigurationDriftRecords",
                column: "BaselineBackupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDriftRecords_CurrentBackupId",
                table: "ConfigurationDriftRecords",
                column: "CurrentBackupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDriftRecords_DeviceId",
                table: "ConfigurationDriftRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDriftRecords_TenantId_DeviceId_DetectedAtUtc",
                table: "ConfigurationDriftRecords",
                columns: new[] { "TenantId", "DeviceId", "DetectedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDriftRecords_TenantId_HasDrift",
                table: "ConfigurationDriftRecords",
                columns: new[] { "TenantId", "HasDrift" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatDetectionRules_TenantId_IsEnabled",
                table: "ThreatDetectionRules",
                columns: new[] { "TenantId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatDetectionRules_TenantId_ThreatType",
                table: "ThreatDetectionRules",
                columns: new[] { "TenantId", "ThreatType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TargetDeviceId",
                table: "ThreatIndicators",
                column: "TargetDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TenantId_LastDetectedAtUtc",
                table: "ThreatIndicators",
                columns: new[] { "TenantId", "LastDetectedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TenantId_Severity",
                table: "ThreatIndicators",
                columns: new[] { "TenantId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TenantId_SourceIp",
                table: "ThreatIndicators",
                columns: new[] { "TenantId", "SourceIp" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TenantId_TargetDeviceId",
                table: "ThreatIndicators",
                columns: new[] { "TenantId", "TargetDeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_TenantId_ThreatType_Status",
                table: "ThreatIndicators",
                columns: new[] { "TenantId", "ThreatType", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationDriftRecords");

            migrationBuilder.DropTable(
                name: "ThreatDetectionRules");

            migrationBuilder.DropTable(
                name: "ThreatIndicators");
        }
    }
}
