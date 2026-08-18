using MediatR;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Commands.CreateSecurityAdvisory;

public record CreateSecurityAdvisoryCommand(
    string AdvisoryId,
    string Vendor,
    string Title,
    string Summary,
    VulnerabilitySeverity Severity,
    string? RemediationGuidance,
    string? ReferenceUrl,
    DateTime? PublishedAtUtc) : IRequest<SecurityAdvisoryDto>;