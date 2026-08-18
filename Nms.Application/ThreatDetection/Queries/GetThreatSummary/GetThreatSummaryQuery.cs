using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Queries.GetThreatSummary;

public record GetThreatSummaryQuery : IRequest<ThreatSummaryDto>;