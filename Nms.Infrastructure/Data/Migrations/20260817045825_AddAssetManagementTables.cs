using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LifecycleState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LifecycleNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LifecycleChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyProvider = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WarrantyStartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyEndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarrantyContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SiteOrLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RackIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RackUnitPosition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assets_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssignedToUserId",
                table: "Assets",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DeviceId",
                table: "Assets",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_AssetTag",
                table: "Assets",
                columns: new[] { "TenantId", "AssetTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_DeviceId",
                table: "Assets",
                columns: new[] { "TenantId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_LifecycleState",
                table: "Assets",
                columns: new[] { "TenantId", "LifecycleState" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_SerialNumber",
                table: "Assets",
                columns: new[] { "TenantId", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_WarrantyStatus",
                table: "Assets",
                columns: new[] { "TenantId", "WarrantyStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
