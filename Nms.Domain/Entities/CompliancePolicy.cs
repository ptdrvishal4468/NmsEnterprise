using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class CompliancePolicy : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ComplianceCategory Category { get; private set; }
    public ComplianceCheckType CheckType { get; private set; }
    public ComplianceSeverity Severity { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? TargetVendor { get; private set; }
    public DeviceType? TargetDeviceType { get; private set; }
    public string? RuleConfigurationJson { get; private set; }

    // Navigation property
    public ICollection<DeviceComplianceResult> Results { get; private set; } = new List<DeviceComplianceResult>();

    // EF Core materialization constructor
    private CompliancePolicy() { }

    public CompliancePolicy(
        Guid id,
        Guid tenantId,
        string name,
        string description,
        ComplianceCategory category,
        ComplianceCheckType checkType,
        ComplianceSeverity severity,
        bool isActive = true,
        string? targetVendor = null,
        DeviceType? targetDeviceType = null,
        string? ruleConfigurationJson = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Compliance policy name is required.", nameof(name));

        TenantId = tenantId;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Category = category;
        CheckType = checkType;
        Severity = severity;
        IsActive = isActive;
        TargetVendor = string.IsNullOrWhiteSpace(targetVendor) ? null : targetVendor.Trim();
        TargetDeviceType = targetDeviceType;
        RuleConfigurationJson = ruleConfigurationJson;
    }

    public void Update(
        string name,
        string description,
        ComplianceCategory category,
        ComplianceCheckType checkType,
        ComplianceSeverity severity,
        bool isActive,
        string? targetVendor,
        DeviceType? targetDeviceType,
        string? ruleConfigurationJson)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Compliance policy name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Category = category;
        CheckType = checkType;
        Severity = severity;
        IsActive = isActive;
        TargetVendor = string.IsNullOrWhiteSpace(targetVendor) ? null : targetVendor.Trim();
        TargetDeviceType = targetDeviceType;
        RuleConfigurationJson = ruleConfigurationJson;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}