using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class NotificationLogRepository : GenericRepository<NotificationLog, Guid>, INotificationLogRepository
{
    public NotificationLogRepository(NmsDbContext context) : base(context) { }

    public async Task<(IEnumerable<NotificationLog> Items, int TotalCount)> GetPagedLogsAsync(
        Guid tenantId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(l => l.TenantId == tenantId);

        int totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.SentAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}