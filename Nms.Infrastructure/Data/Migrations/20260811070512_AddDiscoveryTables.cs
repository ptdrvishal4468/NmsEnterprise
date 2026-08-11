using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscoveryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscoveryJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IpRange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnmpCommunity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SnmpPort = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalTargets = table.Column<int>(type: "int", nullable: false),
                    ProcessedTargets = table.Column<int>(type: "int", nullable: false),
                    DiscoveredCount = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveryJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscoveredDeviceCandidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscoveryJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    IsIcmpReachable = table.Column<bool>(type: "bit", nullable: false),
                    ResponseTimeMs = table.Column<double>(type: "float", nullable: true),
                    IsSnmpReachable = table.Column<bool>(type: "bit", nullable: false),
                    SysDescr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SysObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SysName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MacAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FingerprintedType = table.Column<int>(type: "int", nullable: false),
                    IdentifiedVendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDuplicate = table.Column<bool>(type: "bit", nullable: false),
                    ExistingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsImported = table.Column<bool>(type: "bit", nullable: false),
                    ImportedDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveredDeviceCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscoveredDeviceCandidates_DiscoveryJobs_DiscoveryJobId",
                        column: x => x.DiscoveryJobId,
                        principalTable: "DiscoveryJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveredDeviceCandidates_DiscoveryJobId",
                table: "DiscoveredDeviceCandidates",
                column: "DiscoveryJobId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveredDeviceCandidates_TenantId_IpAddress",
                table: "DiscoveredDeviceCandidates",
                columns: new[] { "TenantId", "IpAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveryJobs_TenantId_Status",
                table: "DiscoveryJobs",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscoveredDeviceCandidates");

            migrationBuilder.DropTable(
                name: "DiscoveryJobs");
        }
    }
}
