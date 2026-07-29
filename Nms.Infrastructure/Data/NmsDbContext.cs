using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nms.Domain.Common;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data;

public class NmsDbContext : DbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceMetricRaw> DeviceMetricsRaw => Set<DeviceMetricRaw>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public NmsDbContext(DbContextOptions<NmsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply all IEntityTypeConfiguration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}