using MediatR;
using Nms.Application.Ticketing.Dtos;

namespace Nms.Application.Ticketing.Commands.CloseTicket;

public record CloseTicketCommand(Guid Id) : IRequest<TicketDto?>;