using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class NotificationTemplateRepository : GenericRepository<NotificationTemplate, Guid>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(NmsDbContext context) : base(context) { }

    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesByChannelAsync(
        Guid tenantId,
        NotificationChannel channel,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.TenantId == tenantId && t.Channel == channel && t.IsEnabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesForTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.TenantId == tenantId && t.IsEnabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<NotificationTemplate> Items, int TotalCount)> GetPagedTemplatesAsync(
        Guid tenantId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(t => t.TenantId == tenantId);

        int totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}