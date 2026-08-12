using MediatR;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Notifications.Commands.UpdateNotificationTemplate;

public record UpdateNotificationTemplateCommand(
    Guid Id,
    string Name,
    NotificationChannel Channel,
    string SubjectTemplate,
    string BodyTemplate,
    string RecipientTarget) : IRequest<NotificationTemplateDto>;