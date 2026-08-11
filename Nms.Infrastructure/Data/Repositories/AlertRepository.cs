using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class AlertRepository : GenericRepository<Alert, Guid>, IAlertRepository
{
    public AlertRepository(NmsDbContext dbContext) : base(dbContext) { }

    public async Task<Alert?> GetActiveAlertByRuleAndDeviceAsync(Guid alertRuleId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(a =>
                a.AlertRuleId == alertRuleId &&
                a.DeviceId == deviceId &&
                (a.State == AlertState.Active || a.State == AlertState.Acknowledged || a.State == AlertState.Suppressed),
                cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetActiveAlertsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.DeviceId == deviceId && (a.State == AlertState.Active || a.State == AlertState.Acknowledged || a.State == AlertState.Suppressed))
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Alert> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId = null,
        AlertState? state = null,
        AlertSeverity? severity = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(a => a.DeviceId == deviceId.Value);
        }

        if (state.HasValue)
        {
            query = query.Where(a => a.State == state.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(a => a.Severity == severity.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.TriggeredAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}