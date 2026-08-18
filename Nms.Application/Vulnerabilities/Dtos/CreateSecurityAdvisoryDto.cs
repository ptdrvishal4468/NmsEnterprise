using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Dtos;

public record CreateSecurityAdvisoryDto(
    string AdvisoryId,
    string Vendor,
    string Title,
    string Summary,
    VulnerabilitySeverity Severity,
    string? RemediationGuidance,
    string? ReferenceUrl,
    DateTime? PublishedAtUtc);