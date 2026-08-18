using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DeviceComplianceResult : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid ScanId { get; private set; }
    public DeviceComplianceScan? Scan { get; private set; }
    public Guid PolicyId { get; private set; }
    public CompliancePolicy? Policy { get; private set; }
    public ComplianceCheckType CheckType { get; private set; }
    public ComplianceCategory Category { get; private set; }
    public ComplianceSeverity Severity { get; private set; }
    public ComplianceStatus Status { get; private set; }
    public string Summary { get; private set; } = string.Empty;
    public string? Details { get; private set; }
    public string? RemediationGuidance { get; private set; }
    public DateTime EvaluatedAtUtc { get; private set; } = DateTime.UtcNow;

    // EF Core materialization constructor
    private DeviceComplianceResult() { }

    public DeviceComplianceResult(
        Guid id,
        Guid tenantId,
        Guid scanId,
        Guid policyId,
        ComplianceCheckType checkType,
        ComplianceCategory category,
        ComplianceSeverity severity,
        ComplianceStatus status,
        string summary,
        string? details = null,
        string? remediationGuidance = null,
        DateTime? evaluatedAtUtc = null) : base(id)
    {
        if (scanId == Guid.Empty)
            throw new ArgumentException("Valid ScanId is required.", nameof(scanId));

        if (policyId == Guid.Empty)
            throw new ArgumentException("Valid PolicyId is required.", nameof(policyId));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary is required.", nameof(summary));

        TenantId = tenantId;
        ScanId = scanId;
        PolicyId = policyId;
        CheckType = checkType;
        Category = category;
        Severity = severity;
        Status = status;
        Summary = summary.Trim();
        Details = details;
        RemediationGuidance = remediationGuidance;
        EvaluatedAtUtc = evaluatedAtUtc ?? DateTime.UtcNow;
    }
}