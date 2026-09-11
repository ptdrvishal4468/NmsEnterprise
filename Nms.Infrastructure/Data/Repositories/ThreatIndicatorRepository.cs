using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ThreatIndicatorRepository : GenericRepository<ThreatIndicator, Guid>, IThreatIndicatorRepository
{
    public ThreatIndicatorRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<ThreatIndicator?> GetActiveIndicatorAsync(
        Guid tenantId,
        ThreatType type,
        string? sourceIp,
        Guid? targetDeviceId,
        string? targetUser,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(t => t.TenantId == tenantId && t.ThreatType == type && t.Status == ThreatStatus.Active);

        if (!string.IsNullOrWhiteSpace(sourceIp))
            query = query.Where(t => t.SourceIp == sourceIp);

        if (targetDeviceId.HasValue)
            query = query.Where(t => t.TargetDeviceId == targetDeviceId.Value);

        if (!string.IsNullOrWhiteSpace(targetUser))
            query = query.Where(t => t.TargetUser == targetUser);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ThreatIndicator> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        ThreatType? threatType = null,
        ThreatSeverity? severity = null,
        ThreatStatus? status = null,
        Guid? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(t => t.TargetDevice)
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId);

        if (threatType.HasValue)
            query = query.Where(t => t.ThreatType == threatType.Value);

        if (severity.HasValue)
            query = query.Where(t => t.Severity == severity.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (deviceId.HasValue)
            query = query.Where(t => t.TargetDeviceId == deviceId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.LastDetectedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}