using MediatR;
using Nms.Application.Ticketing.Dtos;

namespace Nms.Application.Ticketing.Commands.SyncTicket;

public record SyncTicketCommand(Guid Id) : IRequest<SyncTicketResultDto?>;