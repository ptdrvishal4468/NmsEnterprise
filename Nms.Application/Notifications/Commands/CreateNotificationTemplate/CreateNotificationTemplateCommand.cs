using MediatR;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Notifications.Commands.CreateNotificationTemplate;

public record CreateNotificationTemplateCommand(
    string Name,
    NotificationChannel Channel,
    string SubjectTemplate,
    string BodyTemplate,
    string RecipientTarget) : IRequest<NotificationTemplateDto>;