using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface INotificationDispatcher
{
    Task DispatchAlertNotificationAsync(Alert alert, CancellationToken cancellationToken = default);
}