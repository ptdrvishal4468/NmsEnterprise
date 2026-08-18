using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Queries.GetThreatIndicatorsPaged;

public record GetThreatIndicatorsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    ThreatType? ThreatType = null,
    ThreatSeverity? Severity = null,
    ThreatStatus? Status = null,
    Guid? DeviceId = null) : IRequest<PagedResult<ThreatIndicatorDto>>;