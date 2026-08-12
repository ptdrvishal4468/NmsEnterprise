using MediatR;
using Nms.Application.Events.Dtos;

namespace Nms.Application.Events.Queries.GetEventById;

public record GetEventByIdQuery(Guid Id) : IRequest<EventDto?>;