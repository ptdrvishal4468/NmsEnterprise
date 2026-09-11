using Nms.Domain.Enums;

namespace Nms.Application.Notifications.Dtos;

public record NotificationTemplateDto(
    Guid Id,
    Guid TenantId,
    string Name,
    NotificationChannel Channel,
    string SubjectTemplate,
    string BodyTemplate,
    string RecipientTarget,
    bool IsEnabled,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);