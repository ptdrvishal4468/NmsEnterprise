using MediatR;
using Nms.Application.Vulnerabilities.Dtos;

namespace Nms.Application.Vulnerabilities.Commands.AnalyzeDeviceVulnerabilities;

public record AnalyzeDeviceVulnerabilitiesCommand(Guid DeviceId) : IRequest<IReadOnlyList<DeviceVulnerabilityMatchDto>>;