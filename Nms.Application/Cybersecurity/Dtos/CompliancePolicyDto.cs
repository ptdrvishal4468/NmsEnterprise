using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Dtos;

public class CompliancePolicyDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplianceCategory Category { get; set; }
    public ComplianceCheckType CheckType { get; set; }
    public ComplianceSeverity Severity { get; set; }
    public bool IsActive { get; set; }
    public string? TargetVendor { get; set; }
    public DeviceType? TargetDeviceType { get; set; }
    public string? RuleConfigurationJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}