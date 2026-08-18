using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20260818123812_AddCybersecurityComplianceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompliancePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CheckType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TargetVendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetDeviceType = table.Column<int>(type: "int", nullable: true),
                    RuleConfigurationJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompliancePolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceComplianceScans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScannedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallStatus = table.Column<int>(type: "int", nullable: false),
                    PassedChecks = table.Column<int>(type: "int", nullable: false),
                    FailedChecks = table.Column<int>(type: "int", nullable: false),
                    WarningChecks = table.Column<int>(type: "int", nullable: false),
                    NotApplicableChecks = table.Column<int>(type: "int", nullable: false),
                    TotalChecks = table.Column<int>(type: "int", nullable: false),
                    EvaluationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceComplianceScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceComplianceScans_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceComplianceResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RemediationGuidance = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EvaluatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceComplianceResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceComplianceResults_CompliancePolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "CompliancePolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceComplianceResults_DeviceComplianceScans_ScanId",
                        column: x => x.ScanId,
                        principalTable: "DeviceComplianceScans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompliancePolicies_TenantId_Category",
                table: "CompliancePolicies",
                columns: new[] { "TenantId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_CompliancePolicies_TenantId_CheckType",
                table: "CompliancePolicies",
                columns: new[] { "TenantId", "CheckType" });

            migrationBuilder.CreateIndex(
                name: "IX_CompliancePolicies_TenantId_IsActive",
                table: "CompliancePolicies",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceResults_PolicyId",
                table: "DeviceComplianceResults",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceResults_ScanId",
                table: "DeviceComplianceResults",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceResults_TenantId_PolicyId",
                table: "DeviceComplianceResults",
                columns: new[] { "TenantId", "PolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceResults_TenantId_ScanId",
                table: "DeviceComplianceResults",
                columns: new[] { "TenantId", "ScanId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceResults_TenantId_Status",
                table: "DeviceComplianceResults",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceScans_DeviceId",
                table: "DeviceComplianceScans",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceScans_TenantId_DeviceId",
                table: "DeviceComplianceScans",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceScans_TenantId_OverallStatus",
                table: "DeviceComplianceScans",
                columns: new[] { "TenantId", "OverallStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceComplianceScans_TenantId_ScannedAtUtc",
                table: "DeviceComplianceScans",
                columns: new[] { "TenantId", "ScannedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceComplianceResults");

            migrationBuilder.DropTable(
                name: "CompliancePolicies");

            migrationBuilder.DropTable(
                name: "DeviceComplianceScans");
        }
    }
}
