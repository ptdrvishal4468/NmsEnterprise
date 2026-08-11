using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReachabilityHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmwareVersion",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "Devices",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Devices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Devices",
                type: "varchar(17)",
                unicode: false,
                maxLength: 17,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vendor",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeviceReachabilityHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MinLatencyMs = table.Column<double>(type: "float", nullable: false),
                    MaxLatencyMs = table.Column<double>(type: "float", nullable: false),
                    AvgLatencyMs = table.Column<double>(type: "float", nullable: false),
                    CurrentLatencyMs = table.Column<double>(type: "float", nullable: false),
                    PacketsSent = table.Column<int>(type: "int", nullable: false),
                    PacketsReceived = table.Column<int>(type: "int", nullable: false),
                    PacketLossPercentage = table.Column<double>(type: "float", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceReachabilityHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceReachabilityHistories_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_IsActive",
                table: "Tenants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId_DeviceType",
                table: "Devices",
                columns: new[] { "TenantId", "DeviceType" });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId_IpAddress",
                table: "Devices",
                columns: new[] { "TenantId", "IpAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId_SerialNumber",
                table: "Devices",
                columns: new[] { "TenantId", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceReachabilityHistories_DeviceId_TimestampUtc",
                table: "DeviceReachabilityHistories",
                columns: new[] { "DeviceId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceReachabilityHistories_TenantId",
                table: "DeviceReachabilityHistories",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceReachabilityHistories");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_IsActive",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Devices_TenantId_DeviceType",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_TenantId_IpAddress",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_TenantId_SerialNumber",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "FirmwareVersion",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "Devices");
        }
    }
}
