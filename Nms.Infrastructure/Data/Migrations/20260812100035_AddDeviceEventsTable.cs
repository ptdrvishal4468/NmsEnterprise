using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParentEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceEvents_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvents_DeviceId",
                table: "DeviceEvents",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvents_TenantId",
                table: "DeviceEvents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvents_TenantId_CorrelationId",
                table: "DeviceEvents",
                columns: new[] { "TenantId", "CorrelationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvents_TenantId_CreatedAtUtc",
                table: "DeviceEvents",
                columns: new[] { "TenantId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvents_TenantId_DeviceId",
                table: "DeviceEvents",
                columns: new[] { "TenantId", "DeviceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceEvents");
        }
    }
}
