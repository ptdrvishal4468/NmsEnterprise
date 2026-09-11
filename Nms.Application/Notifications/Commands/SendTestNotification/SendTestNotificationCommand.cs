using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Notifications.Commands.SendTestNotification;

public record SendTestNotificationCommand(
    NotificationChannel Channel,
    string RecipientTarget,
    string Subject,
    string Body) : IRequest<bool>;