using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;

public record EvaluateDeviceComplianceCommand(
    Guid DeviceId,
    string? EvaluationNotes = null,
    IReadOnlyList<Guid>? SpecificPolicyIds = null) : IRequest<DeviceComplianceScanDto?>;