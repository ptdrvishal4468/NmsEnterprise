using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Dtos;

public record UpdateTicketDto(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    string? MetadataJson = null);