using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeUnauthorizedAccess;

public record AnalyzeUnauthorizedAccessCommand(int TimeWindowMinutes = 60) : IRequest<IReadOnlyList<ThreatIndicatorDto>>;