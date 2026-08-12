using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface INotificationProvider
{
    NotificationChannel Channel { get; }
    Task<bool> SendAsync(
        string recipientTarget,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}