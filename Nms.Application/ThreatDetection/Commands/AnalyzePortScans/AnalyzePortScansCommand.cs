using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzePortScans;

public record AnalyzePortScansCommand(int TimeWindowMinutes = 60) : IRequest<IReadOnlyList<ThreatIndicatorDto>>;