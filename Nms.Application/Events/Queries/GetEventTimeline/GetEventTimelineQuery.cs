using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Events.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Events.Queries.GetEventTimeline;

public record GetEventTimelineQuery(
    Guid DeviceId,
    int PageNumber = 1,
    int PageSize = 50,
    EventCategory? Category = null,
    EventSeverity? Severity = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<PagedResult<EventDto>>;