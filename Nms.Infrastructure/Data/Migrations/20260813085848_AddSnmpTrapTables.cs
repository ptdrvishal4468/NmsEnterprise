using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSnmpTrapTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SnmpTrapMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SnmpVersion = table.Column<int>(type: "int", nullable: false),
                    Community = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnterpriseOid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GenericTrap = table.Column<int>(type: "int", nullable: true),
                    SpecificTrap = table.Column<int>(type: "int", nullable: true),
                    TrapOid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AgentAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    SourceIpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    VarbindsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawPayloadHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMalformed = table.Column<bool>(type: "bit", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnmpTrapMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnmpTrapMessages_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_DeviceId",
                table: "SnmpTrapMessages",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId",
                table: "SnmpTrapMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId_DeviceId",
                table: "SnmpTrapMessages",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId_EnterpriseOid",
                table: "SnmpTrapMessages",
                columns: new[] { "TenantId", "EnterpriseOid" });

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId_Severity",
                table: "SnmpTrapMessages",
                columns: new[] { "TenantId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId_SourceIpAddress",
                table: "SnmpTrapMessages",
                columns: new[] { "TenantId", "SourceIpAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_SnmpTrapMessages_TenantId_TimestampUtc",
                table: "SnmpTrapMessages",
                columns: new[] { "TenantId", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SnmpTrapMessages");
        }
    }
}
