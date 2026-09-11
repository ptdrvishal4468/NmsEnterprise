using MediatR;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;

public record UpdateThreatIndicatorStatusCommand(
    Guid Id,
    ThreatStatus Status,
    string? ResolutionNotes = null) : IRequest<ThreatIndicatorDto>;