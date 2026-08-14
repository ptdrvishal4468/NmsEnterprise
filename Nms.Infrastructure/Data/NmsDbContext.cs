using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Common;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data;

public class NmsDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<PollProfile> PollProfiles => Set<PollProfile>();
    public DbSet<DeviceMetricRaw> DeviceMetricsRaw => Set<DeviceMetricRaw>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<DeviceReachabilityHistory> DeviceReachabilityHistories => Set<DeviceReachabilityHistory>();
    public DbSet<DiscoveryJob> DiscoveryJobs => Set<DiscoveryJob>();
    public DbSet<DiscoveredDeviceCandidate> DiscoveredDeviceCandidates => Set<DiscoveredDeviceCandidate>();
    public DbSet<NetworkInterface> NetworkInterfaces => Set<NetworkInterface>();
    public DbSet<NetworkInterfaceHistory> NetworkInterfaceHistories => Set<NetworkInterfaceHistory>();
    public DbSet<DeviceHealthHistory> DeviceHealthHistories => Set<DeviceHealthHistory>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<AlertHistory> AlertHistories => Set<AlertHistory>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<DeviceEvent> DeviceEvents => Set<DeviceEvent>();
    public DbSet<SyslogMessage> SyslogMessages => Set<SyslogMessage>();
    public DbSet<SnmpTrapMessage> SnmpTrapMessages => Set<SnmpTrapMessage>();
    public DbSet<ConfigurationBackup> ConfigurationBackups => Set<ConfigurationBackup>();
    public DbSet<BackupSchedule> BackupSchedules => Set<BackupSchedule>();
    public DbSet<ConfigurationRestoreLog> ConfigurationRestoreLogs => Set<ConfigurationRestoreLog>();
    public DbSet<FirmwareBaseline> FirmwareBaselines => Set<FirmwareBaseline>();
    public DbSet<FirmwareUpgradePlan> FirmwareUpgradePlans => Set<FirmwareUpgradePlan>();
    public DbSet<TopologyLink> TopologyLinks => Set<TopologyLink>();

    public NmsDbContext(
        DbContextOptions<NmsDbContext> options,
        ITenantContext? tenantContext = null) : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply all IEntityTypeConfiguration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply Global Multi-Tenant Query Filter on all entities implementing IMustHaveTenant
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(NmsDbContext)
                    .GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, IMustHaveTenant
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => _tenantContext != null && _tenantContext.IsResolved && e.TenantId == _tenantContext.TenantId);
    }
}