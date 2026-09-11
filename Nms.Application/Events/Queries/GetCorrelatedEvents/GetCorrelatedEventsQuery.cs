using MediatR;
using Nms.Application.Events.Dtos;

namespace Nms.Application.Events.Queries.GetCorrelatedEvents;

public record GetCorrelatedEventsQuery(string CorrelationId) : IRequest<IReadOnlyList<EventDto>>;