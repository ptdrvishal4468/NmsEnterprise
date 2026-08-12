using MediatR;

namespace Nms.Application.Notifications.Commands.DeleteNotificationTemplate;

public record DeleteNotificationTemplateCommand(Guid Id) : IRequest;