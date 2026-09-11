using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPollProfilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceMetricsRaw_DeviceId",
                table: "DeviceMetricsRaw");

            migrationBuilder.AddColumn<Guid>(
                name: "PollProfileId",
                table: "Devices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiskUtilization",
                table: "DeviceMetricsRaw",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FanStatus",
                table: "DeviceMetricsRaw",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InterfaceUtilization",
                table: "DeviceMetricsRaw",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PowerSupplyStatus",
                table: "DeviceMetricsRaw",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "DeviceMetricsRaw",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "DeviceMetricsRaw",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PollProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IntervalSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_PollProfileId",
                table: "Devices",
                column: "PollProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMetricsRaw_TenantId_DeviceId_TimestampUtc",
                table: "DeviceMetricsRaw",
                columns: new[] { "TenantId", "DeviceId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PollProfiles_TenantId_IsEnabled",
                table: "PollProfiles",
                columns: new[] { "TenantId", "IsEnabled" });

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_PollProfiles_PollProfileId",
                table: "Devices",
                column: "PollProfileId",
                principalTable: "PollProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_PollProfiles_PollProfileId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "PollProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Devices_PollProfileId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_DeviceMetricsRaw_TenantId_DeviceId_TimestampUtc",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "PollProfileId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DiskUtilization",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "FanStatus",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "InterfaceUtilization",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "PowerSupplyStatus",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DeviceMetricsRaw");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMetricsRaw_DeviceId",
                table: "DeviceMetricsRaw",
                column: "DeviceId");
        }
    }
}
