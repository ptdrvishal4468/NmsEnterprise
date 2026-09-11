using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class FirmwareUpgradeRecommendation : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public string CurrentVersion { get; private set; } = string.Empty;
    public string RecommendedVersion { get; private set; } = string.Empty;
    public RecommendationPriority Priority { get; private set; } = RecommendationPriority.Medium;
    public string Reasoning { get; private set; } = string.Empty;
    public int AssociatedVulnerabilityCount { get; private set; }
    public int CriticalVulnerabilityCount { get; private set; }
    public bool IsApplied { get; private set; }

    // Navigation property
    public Device? Device { get; private set; }

    // EF Core private constructor
    private FirmwareUpgradeRecommendation() { }

    public FirmwareUpgradeRecommendation(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        string currentVersion,
        string recommendedVersion,
        RecommendationPriority priority,
        string reasoning,
        int associatedVulnerabilityCount,
        int criticalVulnerabilityCount) : base(id)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(deviceId));

        if (string.IsNullOrWhiteSpace(recommendedVersion))
            throw new ArgumentException("Recommended version is required.", nameof(recommendedVersion));

        TenantId = tenantId;
        DeviceId = deviceId;
        CurrentVersion = currentVersion?.Trim() ?? string.Empty;
        RecommendedVersion = recommendedVersion.Trim();
        Priority = priority;
        Reasoning = reasoning?.Trim() ?? string.Empty;
        AssociatedVulnerabilityCount = associatedVulnerabilityCount;
        CriticalVulnerabilityCount = criticalVulnerabilityCount;
        IsApplied = false;
    }

    public void MarkAsApplied()
    {
        IsApplied = true;
    }
}