using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class EventRepository : GenericRepository<DeviceEvent, Guid>, IEventRepository
{
    public EventRepository(NmsDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<DeviceEvent> Items, int TotalCount)> GetEventsPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        Guid? deviceId = null,
        EventCategory? category = null,
        EventSeverity? severity = null,
        string? correlationId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DeviceEvent> query = DbSet
            .AsNoTracking()
            .Where(e => e.TenantId == tenantId);

        if (deviceId.HasValue)
            query = query.Where(e => e.DeviceId == deviceId.Value);

        if (category.HasValue)
            query = query.Where(e => e.Category == category.Value);

        if (severity.HasValue)
            query = query.Where(e => e.Severity == severity.Value);

        if (!string.IsNullOrWhiteSpace(correlationId))
            query = query.Where(e => e.CorrelationId == correlationId);

        if (fromUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc <= toUtc.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<DeviceEvent>> GetCorrelatedEventsAsync(
        Guid tenantId,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(e => e.TenantId == tenantId && e.CorrelationId == correlationId)
            .OrderBy(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}