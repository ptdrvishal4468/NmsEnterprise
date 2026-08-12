using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface INotificationLogRepository : IGenericRepository<NotificationLog, Guid>
{
    Task<(IEnumerable<NotificationLog> Items, int TotalCount)> GetPagedLogsAsync(
        Guid tenantId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}