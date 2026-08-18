using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;

public record UpdateCompliancePolicyCommand(
    Guid Id,
    string Name,
    string Description,
    ComplianceCategory Category,
    ComplianceCheckType CheckType,
    ComplianceSeverity Severity,
    bool IsActive,
    string? TargetVendor = null,
    DeviceType? TargetDeviceType = null,
    string? RuleConfigurationJson = null) : IRequest<CompliancePolicyDto?>;