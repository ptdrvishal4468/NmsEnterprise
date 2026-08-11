using Nms.Domain.Enums;

namespace Nms.Application.Alerts.Dtos;

public record AlertHistoryDto(
    Guid Id,
    Guid TenantId,
    Guid AlertId,
    AlertState OldState,
    AlertState NewState,
    string ChangedBy,
    string Note,
    DateTime TimestampUtc);