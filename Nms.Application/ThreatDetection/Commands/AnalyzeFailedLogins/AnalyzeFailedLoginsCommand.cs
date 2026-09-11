using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeFailedLogins;

public record AnalyzeFailedLoginsCommand(int? ThresholdOverride = null, int? TimeWindowMinutesOverride = null) : IRequest<IReadOnlyList<ThreatIndicatorDto>>;