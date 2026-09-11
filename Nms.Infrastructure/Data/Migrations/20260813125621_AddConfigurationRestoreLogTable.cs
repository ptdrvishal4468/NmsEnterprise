using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationRestoreLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationRestoreLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetBackupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreRestoreBackupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InitiatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RollbackReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AuditNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationRestoreLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationRestoreLogs_ConfigurationBackups_PreRestoreBackupId",
                        column: x => x.PreRestoreBackupId,
                        principalTable: "ConfigurationBackups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConfigurationRestoreLogs_ConfigurationBackups_TargetBackupId",
                        column: x => x.TargetBackupId,
                        principalTable: "ConfigurationBackups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationRestoreLogs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_DeviceId",
                table: "ConfigurationRestoreLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_PreRestoreBackupId",
                table: "ConfigurationRestoreLogs",
                column: "PreRestoreBackupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_TargetBackupId",
                table: "ConfigurationRestoreLogs",
                column: "TargetBackupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_TenantId",
                table: "ConfigurationRestoreLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_TenantId_DeviceId",
                table: "ConfigurationRestoreLogs",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationRestoreLogs_TenantId_Status",
                table: "ConfigurationRestoreLogs",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationRestoreLogs");
        }
    }
}
