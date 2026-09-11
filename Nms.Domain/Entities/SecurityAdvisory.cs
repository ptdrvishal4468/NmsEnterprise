using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class SecurityAdvisory : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string AdvisoryId { get; private set; } = string.Empty;
    public string Vendor { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public VulnerabilitySeverity Severity { get; private set; } = VulnerabilitySeverity.Medium;
    public string? RemediationGuidance { get; private set; }
    public string? ReferenceUrl { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }

    // EF Core private constructor
    private SecurityAdvisory() { }

    public SecurityAdvisory(
        Guid id,
        Guid tenantId,
        string advisoryId,
        string vendor,
        string title,
        string summary,
        VulnerabilitySeverity severity,
        string? remediationGuidance = null,
        string? referenceUrl = null,
        DateTime? publishedAtUtc = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(advisoryId))
            throw new ArgumentException("Advisory ID is required.", nameof(advisoryId));

        if (string.IsNullOrWhiteSpace(vendor))
            throw new ArgumentException("Vendor is required.", nameof(vendor));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        TenantId = tenantId;
        AdvisoryId = advisoryId.Trim().ToUpperInvariant();
        Vendor = vendor.Trim();
        Title = title.Trim();
        Summary = summary?.Trim() ?? string.Empty;
        Severity = severity;
        RemediationGuidance = remediationGuidance?.Trim();
        ReferenceUrl = referenceUrl?.Trim();
        PublishedAtUtc = publishedAtUtc;
    }

    public void Update(
        string vendor,
        string title,
        string summary,
        VulnerabilitySeverity severity,
        string? remediationGuidance,
        string? referenceUrl,
        DateTime? publishedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(vendor))
            throw new ArgumentException("Vendor is required.", nameof(vendor));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Vendor = vendor.Trim();
        Title = title.Trim();
        Summary = summary?.Trim() ?? string.Empty;
        Severity = severity;
        RemediationGuidance = remediationGuidance?.Trim();
        ReferenceUrl = referenceUrl?.Trim();
        PublishedAtUtc = publishedAtUtc;
    }
}