using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Dtos;

public class CreateCompliancePolicyDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplianceCategory Category { get; set; }
    public ComplianceCheckType CheckType { get; set; }
    public ComplianceSeverity Severity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? TargetVendor { get; set; }
    public DeviceType? TargetDeviceType { get; set; }
    public string? RuleConfigurationJson { get; set; }
}