using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog, long>, IAuditLogRepository
{
    public AuditLogRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> SearchAuditLogsAsync(
        Guid tenantId,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? userId,
        string? action,
        AuditCategory? category,
        AuditStatus? status,
        string? entityName,
        string? entityId,
        string? searchTerm,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId);

        if (fromUtc.HasValue)
            query = query.Where(a => a.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.TimestampUtc <= toUtc.Value);

        if (userId.HasValue && userId.Value != Guid.Empty)
            query = query.Where(a => a.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (category.HasValue)
            query = query.Where(a => a.Category == category.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        if (!string.IsNullOrWhiteSpace(entityId))
            query = query.Where(a => a.EntityId == entityId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(a => a.Action.Contains(term) ||
                                     (a.Username != null && a.Username.Contains(term)) ||
                                     (a.EntityName != null && a.EntityName.Contains(term)) ||
                                     (a.Details != null && a.Details.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.TimestampUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<AuditLog>> GetConfigurationChangesAsync(
        Guid tenantId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? entityName,
        int maxCount,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId &&
                       (a.Category == AuditCategory.Configuration || a.OldValuesJson != null || a.NewValuesJson != null));

        if (fromUtc.HasValue)
            query = query.Where(a => a.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.TimestampUtc <= toUtc.Value);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        return await query
            .OrderByDescending(a => a.TimestampUtc)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetUserActivityAsync(
        Guid tenantId,
        Guid? userId,
        DateTime? fromUtc,
        DateTime? toUtc,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId);

        if (userId.HasValue && userId.Value != Guid.Empty)
            query = query.Where(a => a.UserId == userId.Value);

        if (fromUtc.HasValue)
            query = query.Where(a => a.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.TimestampUtc <= toUtc.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.TimestampUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}