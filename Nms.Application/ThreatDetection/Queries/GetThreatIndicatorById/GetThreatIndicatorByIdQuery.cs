using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Queries.GetThreatIndicatorById;

public record GetThreatIndicatorByIdQuery(Guid Id) : IRequest<ThreatIndicatorDto?>;