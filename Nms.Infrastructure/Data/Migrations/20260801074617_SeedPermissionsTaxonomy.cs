using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermissionsTaxonomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceMetricsRaw_Devices_DeviceId",
                table: "DeviceMetricsRaw");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceMetricsRaw",
                table: "DeviceMetricsRaw");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceMetricsRaw",
                table: "DeviceMetricsRaw",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "PermissionKey" },
                values: new object[,]
                {
                    { 1, "View user details and lists", "Users.View" },
                    { 2, "Create new user accounts", "Users.Create" },
                    { 3, "Update existing user profiles", "Users.Update" },
                    { 4, "Delete user accounts", "Users.Delete" },
                    { 5, "Assign or update user roles", "Users.ManageRoles" },
                    { 6, "View existing roles and mappings", "Roles.View" },
                    { 7, "Create new security roles", "Roles.Create" },
                    { 8, "Update existing role names", "Roles.Update" },
                    { 9, "Delete custom security roles", "Roles.Delete" },
                    { 10, "Assign permissions to roles", "Roles.AssignPermissions" },
                    { 11, "View inventory devices and status", "Devices.View" },
                    { 12, "Add new network devices", "Devices.Create" },
                    { 13, "Update network device configurations", "Devices.Update" },
                    { 14, "Remove devices from inventory", "Devices.Delete" },
                    { 15, "Execute management commands on devices", "Devices.Control" },
                    { 16, "View raw and processed device telemetry", "Telemetry.View" },
                    { 17, "Trigger manual SNMP device polling", "Telemetry.Poll" },
                    { 18, "Export telemetry reports and metric logs", "Telemetry.Export" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceMetricsRaw",
                table: "DeviceMetricsRaw");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceMetricsRaw",
                table: "DeviceMetricsRaw",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceMetricsRaw_Devices_DeviceId",
                table: "DeviceMetricsRaw",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
