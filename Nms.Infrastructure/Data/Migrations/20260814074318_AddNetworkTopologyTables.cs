using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNetworkTopologyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopologyLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceInterfaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetInterfaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LayerType = table.Column<int>(type: "int", nullable: false),
                    Protocol = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SpeedBps = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    LastDiscoveredUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopologyLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopologyLinks_Devices_SourceDeviceId",
                        column: x => x.SourceDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopologyLinks_Devices_TargetDeviceId",
                        column: x => x.TargetDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopologyLinks_NetworkInterfaces_SourceInterfaceId",
                        column: x => x.SourceInterfaceId,
                        principalTable: "NetworkInterfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopologyLinks_NetworkInterfaces_TargetInterfaceId",
                        column: x => x.TargetInterfaceId,
                        principalTable: "NetworkInterfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_SourceDeviceId",
                table: "TopologyLinks",
                column: "SourceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_SourceInterfaceId",
                table: "TopologyLinks",
                column: "SourceInterfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TargetDeviceId",
                table: "TopologyLinks",
                column: "TargetDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TargetInterfaceId",
                table: "TopologyLinks",
                column: "TargetInterfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TenantId",
                table: "TopologyLinks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TenantId_LayerType_Status",
                table: "TopologyLinks",
                columns: new[] { "TenantId", "LayerType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TenantId_SourceDeviceId",
                table: "TopologyLinks",
                columns: new[] { "TenantId", "SourceDeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TenantId_Status",
                table: "TopologyLinks",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TopologyLinks_TenantId_TargetDeviceId",
                table: "TopologyLinks",
                columns: new[] { "TenantId", "TargetDeviceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopologyLinks");
        }
    }
}
