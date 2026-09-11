using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class FirmwareUpgradePlan : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public string TargetVersion { get; private set; } = string.Empty;
    public DateTime PlannedDateUtc { get; private set; }
    public UpgradePlanStatus Status { get; private set; } = UpgradePlanStatus.Planned;
    public string? Notes { get; private set; }

    // Navigation property
    public Device? Device { get; private set; }

    // EF Core private constructor
    private FirmwareUpgradePlan() { }

    public FirmwareUpgradePlan(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        string targetVersion,
        DateTime plannedDateUtc,
        string? notes = null) : base(id)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(deviceId));

        if (string.IsNullOrWhiteSpace(targetVersion))
            throw new ArgumentException("Target version is required.", nameof(targetVersion));

        TenantId = tenantId;
        DeviceId = deviceId;
        TargetVersion = targetVersion.Trim();
        PlannedDateUtc = plannedDateUtc;
        Notes = notes;
        Status = UpgradePlanStatus.Planned;
    }

    public void UpdateStatus(UpgradePlanStatus newStatus)
    {
        Status = newStatus;
    }

    public void UpdatePlanDetails(string targetVersion, DateTime plannedDateUtc, string? notes)
    {
        if (string.IsNullOrWhiteSpace(targetVersion))
            throw new ArgumentException("Target version is required.", nameof(targetVersion));

        TargetVersion = targetVersion.Trim();
        PlannedDateUtc = plannedDateUtc;
        Notes = notes;
    }
}