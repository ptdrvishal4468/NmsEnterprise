using MediatR;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Commands.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    string? MetadataJson = null) : IRequest<TicketDto?>;