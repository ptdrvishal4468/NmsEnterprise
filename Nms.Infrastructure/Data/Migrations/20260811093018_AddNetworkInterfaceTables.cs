using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNetworkInterfaceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetworkInterfaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IfIndex = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    InterfaceType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    MacAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SpeedBps = table.Column<long>(type: "bigint", nullable: false),
                    AdminStatus = table.Column<int>(type: "int", nullable: false),
                    OperStatus = table.Column<int>(type: "int", nullable: false),
                    Duplex = table.Column<int>(type: "int", nullable: false),
                    InOctets = table.Column<long>(type: "bigint", nullable: false),
                    OutOctets = table.Column<long>(type: "bigint", nullable: false),
                    InErrors = table.Column<long>(type: "bigint", nullable: false),
                    OutErrors = table.Column<long>(type: "bigint", nullable: false),
                    InDiscards = table.Column<long>(type: "bigint", nullable: false),
                    OutDiscards = table.Column<long>(type: "bigint", nullable: false),
                    UtilizationPercent = table.Column<double>(type: "float", nullable: false),
                    LastPolledUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkInterfaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkInterfaces_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkInterfaceHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkInterfaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IfIndex = table.Column<int>(type: "int", nullable: false),
                    AdminStatus = table.Column<int>(type: "int", nullable: false),
                    OperStatus = table.Column<int>(type: "int", nullable: false),
                    SpeedBps = table.Column<long>(type: "bigint", nullable: false),
                    InOctets = table.Column<long>(type: "bigint", nullable: false),
                    OutOctets = table.Column<long>(type: "bigint", nullable: false),
                    InErrors = table.Column<long>(type: "bigint", nullable: false),
                    OutErrors = table.Column<long>(type: "bigint", nullable: false),
                    InDiscards = table.Column<long>(type: "bigint", nullable: false),
                    OutDiscards = table.Column<long>(type: "bigint", nullable: false),
                    UtilizationPercent = table.Column<double>(type: "float", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkInterfaceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkInterfaceHistory_NetworkInterfaces_NetworkInterfaceId",
                        column: x => x.NetworkInterfaceId,
                        principalTable: "NetworkInterfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInterfaceHistory_NetworkInterfaceId_TimestampUtc",
                table: "NetworkInterfaceHistory",
                columns: new[] { "NetworkInterfaceId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInterfaceHistory_TenantId_DeviceId_TimestampUtc",
                table: "NetworkInterfaceHistory",
                columns: new[] { "TenantId", "DeviceId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInterfaces_DeviceId",
                table: "NetworkInterfaces",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInterfaces_TenantId_DeviceId",
                table: "NetworkInterfaces",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInterfaces_TenantId_DeviceId_IfIndex",
                table: "NetworkInterfaces",
                columns: new[] { "TenantId", "DeviceId", "IfIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkInterfaceHistory");

            migrationBuilder.DropTable(
                name: "NetworkInterfaces");
        }
    }
}
