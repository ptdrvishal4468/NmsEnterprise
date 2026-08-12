using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate, Guid>
{
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesByChannelAsync(
        Guid tenantId,
        NotificationChannel channel,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesForTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<NotificationTemplate> Items, int TotalCount)> GetPagedTemplatesAsync(
        Guid tenantId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}