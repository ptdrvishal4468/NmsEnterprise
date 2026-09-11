using MediatR;
using Nms.Application.Ticketing.Dtos;

namespace Nms.Application.Ticketing.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDto?>;