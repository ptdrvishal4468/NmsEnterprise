using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Commands.EvaluateAllDevicesCompliance;

public record EvaluateAllDevicesComplianceCommand(string? EvaluationNotes = null) : IRequest<IReadOnlyList<DeviceComplianceScanDto>>;