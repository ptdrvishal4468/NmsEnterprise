using Nms.Domain.Enums;

namespace Nms.Application.Notifications.Dtos;

public record NotificationLogDto(
    Guid Id,
    Guid TenantId,
    Guid? AlertId,
    NotificationChannel Channel,
    string RecipientTarget,
    string Subject,
    string Message,
    NotificationStatus Status,
    string? ErrorMessage,
    DateTime SentAtUtc);