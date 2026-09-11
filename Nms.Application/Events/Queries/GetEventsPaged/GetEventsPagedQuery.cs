using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Events.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Events.Queries.GetEventsPaged;

public record GetEventsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? DeviceId = null,
    EventCategory? Category = null,
    EventSeverity? Severity = null,
    string? CorrelationId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<PagedResult<EventDto>>;