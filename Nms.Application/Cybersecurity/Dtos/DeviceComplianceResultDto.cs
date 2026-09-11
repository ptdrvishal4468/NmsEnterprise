using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Dtos;

public class DeviceComplianceResultDto
{
    public Guid Id { get; set; }
    public Guid ScanId { get; set; }
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public ComplianceCheckType CheckType { get; set; }
    public ComplianceCategory Category { get; set; }
    public ComplianceSeverity Severity { get; set; }
    public ComplianceStatus Status { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? RemediationGuidance { get; set; }
    public DateTime EvaluatedAtUtc { get; set; }
}