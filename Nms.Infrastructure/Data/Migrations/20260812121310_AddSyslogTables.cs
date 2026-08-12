using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSyslogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyslogMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Facility = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SeverityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hostname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AppTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProcessId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceIpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    IsMalformed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyslogMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyslogMessages_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "DeviceId",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "Facility",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_DeviceId",
                table: "SyslogMessages",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SyslogMessages_TenantId",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "Severity",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "SourceIpAddress",
                table: "SyslogMessages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "TimestampUtc",
                table: "SyslogMessages",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyslogMessages");
        }
    }
}
