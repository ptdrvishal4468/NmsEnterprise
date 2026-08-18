using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Commands.CreateCompliancePolicy;

public record CreateCompliancePolicyCommand(
    string Name,
    string Description,
    ComplianceCategory Category,
    ComplianceCheckType CheckType,
    ComplianceSeverity Severity,
    bool IsActive = true,
    string? TargetVendor = null,
    DeviceType? TargetDeviceType = null,
    string? RuleConfigurationJson = null) : IRequest<CompliancePolicyDto>;