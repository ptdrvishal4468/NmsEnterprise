using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationBackupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackupSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastRunUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackupSchedules_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationBackups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ChecksumSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriggerType = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEligibleForRestore = table.Column<bool>(type: "bit", nullable: false),
                    RestorePreparationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationBackups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationBackups_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackupSchedules_DeviceId",
                table: "BackupSchedules",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_BackupSchedules_IsEnabled",
                table: "BackupSchedules",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_BackupSchedules_NextRunUtc",
                table: "BackupSchedules",
                column: "NextRunUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BackupSchedules_TenantId",
                table: "BackupSchedules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationBackups_DeviceId",
                table: "ConfigurationBackups",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationBackups_TenantId",
                table: "ConfigurationBackups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationBackups_TenantId_DeviceId",
                table: "ConfigurationBackups",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationBackups_TenantId_DeviceId_VersionNumber",
                table: "ConfigurationBackups",
                columns: new[] { "TenantId", "DeviceId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationBackups_TenantId_TimestampUtc",
                table: "ConfigurationBackups",
                columns: new[] { "TenantId", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackupSchedules");

            migrationBuilder.DropTable(
                name: "ConfigurationBackups");
        }
    }
}
