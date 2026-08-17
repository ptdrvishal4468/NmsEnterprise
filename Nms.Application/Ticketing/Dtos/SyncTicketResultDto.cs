using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Dtos;

public record SyncTicketResultDto(
    Guid TicketId,
    bool IsSuccess,
    string? ExternalTicketId,
    string? ExternalStatus,
    string? ErrorMessage,
    DateTime SyncedAtUtc);