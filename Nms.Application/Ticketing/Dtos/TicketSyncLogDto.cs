using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Dtos;

public record TicketSyncLogDto(
    Guid Id,
    Guid TicketId,
    TicketingProviderType ProviderType,
    TicketSyncDirection Direction,
    TicketSyncStatus Status,
    string? RequestPayload,
    string? ResponsePayload,
    string? ErrorMessage,
    DateTime TimestampUtc);