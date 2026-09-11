using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Constants;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PermissionKey)
               .IsRequired()
               .HasMaxLength(100)
               .IsUnicode(false);

        builder.HasIndex(p => p.PermissionKey)
               .IsUnique();

        builder.Property(p => p.Description)
               .HasMaxLength(250);

        // --- Seed Permissions ---
        builder.HasData(
            new Permission(1, Permissions.Users.View, "View user details and lists"),
            new Permission(2, Permissions.Users.Create, "Create new user accounts"),
            new Permission(3, Permissions.Users.Update, "Update existing user profiles"),
            new Permission(4, Permissions.Users.Delete, "Delete user accounts"),
            new Permission(5, Permissions.Users.ManageRoles, "Assign or update user roles"),

            new Permission(6, Permissions.Roles.View, "View existing roles and mappings"),
            new Permission(7, Permissions.Roles.Create, "Create new security roles"),
            new Permission(8, Permissions.Roles.Update, "Update existing role names"),
            new Permission(9, Permissions.Roles.Delete, "Delete custom security roles"),
            new Permission(10, Permissions.Roles.AssignPermissions, "Assign permissions to roles"),

            new Permission(11, Permissions.Devices.View, "View inventory devices and status"),
            new Permission(12, Permissions.Devices.Create, "Add new network devices"),
            new Permission(13, Permissions.Devices.Update, "Update network device configurations"),
            new Permission(14, Permissions.Devices.Delete, "Remove devices from inventory"),
            new Permission(15, Permissions.Devices.Control, "Execute management commands on devices"),

            new Permission(16, Permissions.Telemetry.View, "View raw and processed device telemetry"),
            new Permission(17, Permissions.Telemetry.Poll, "Trigger manual SNMP device polling"),
            new Permission(18, Permissions.Telemetry.Export, "Export telemetry reports and metric logs")
        );
    }
}