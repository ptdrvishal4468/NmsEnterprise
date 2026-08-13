using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class BackupSchedule : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Target device scope (null DeviceId means tenant-wide schedule for all SSH devices)
    public Guid? DeviceId { get; private set; }
    public Device? Device { get; private set; }

    public int IntervalMinutes { get; private set; }
    public bool IsEnabled { get; private set; } = true;

    public DateTime? LastRunUtc { get; private set; }
    public DateTime NextRunUtc { get; private set; }

    // Private constructor for EF Core
    private BackupSchedule() { }

    public BackupSchedule(
        Guid id,
        Guid tenantId,
        string name,
        int intervalMinutes,
        Guid? deviceId = null,
        string? description = null) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Schedule name is required.", nameof(name));

        if (intervalMinutes < 5)
            throw new ArgumentException("Backup interval must be at least 5 minutes.", nameof(intervalMinutes));

        TenantId = tenantId;
        Name = name;
        IntervalMinutes = intervalMinutes;
        DeviceId = deviceId;
        Description = description;
        IsEnabled = true;
        NextRunUtc = DateTime.UtcNow.AddMinutes(intervalMinutes);
    }

    public void UpdateSchedule(string name, int intervalMinutes, Guid? deviceId, string? description, bool isEnabled)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Schedule name is required.", nameof(name));

        if (intervalMinutes < 5)
            throw new ArgumentException("Backup interval must be at least 5 minutes.", nameof(intervalMinutes));

        Name = name;
        IntervalMinutes = intervalMinutes;
        DeviceId = deviceId;
        Description = description;
        IsEnabled = isEnabled;
        NextRunUtc = DateTime.UtcNow.AddMinutes(intervalMinutes);
    }

    public void RecordExecution()
    {
        LastRunUtc = DateTime.UtcNow;
        NextRunUtc = DateTime.UtcNow.AddMinutes(IntervalMinutes);
    }

    public void Toggle(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}