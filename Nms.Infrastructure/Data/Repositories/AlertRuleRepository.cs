using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class AlertRuleRepository : GenericRepository<AlertRule, Guid>, IAlertRuleRepository
{
    public AlertRuleRepository(NmsDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<AlertRule>> GetActiveRulesForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.IsEnabled && (r.DeviceId == null || r.DeviceId == deviceId))
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<AlertRule> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(r => r.DeviceId == deviceId.Value || r.DeviceId == null);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}