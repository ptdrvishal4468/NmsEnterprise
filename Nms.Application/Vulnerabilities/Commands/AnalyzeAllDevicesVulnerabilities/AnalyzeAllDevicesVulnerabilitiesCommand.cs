using MediatR;
using Nms.Application.Vulnerabilities.Dtos;

namespace Nms.Application.Vulnerabilities.Commands.AnalyzeAllDevicesVulnerabilities;

public record AnalyzeAllDevicesVulnerabilitiesCommand : IRequest<FirmwareRiskSummaryDto>;