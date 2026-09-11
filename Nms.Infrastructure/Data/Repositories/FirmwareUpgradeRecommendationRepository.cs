using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class FirmwareUpgradeRecommendationRepository : GenericRepository<FirmwareUpgradeRecommendation, Guid>, IFirmwareUpgradeRecommendationRepository
{
    public FirmwareUpgradeRecommendationRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<FirmwareUpgradeRecommendation>> GetActiveByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId && r.DeviceId == deviceId && !r.IsApplied)
            .OrderByDescending(r => r.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<FirmwareUpgradeRecommendation> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? deviceId = null,
        RecommendationPriority? priority = null,
        bool? isApplied = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(r => r.Device)
            .Where(r => r.TenantId == tenantId);

        if (deviceId.HasValue)
        {
            query = query.Where(r => r.DeviceId == deviceId.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(r => r.Priority == priority.Value);
        }

        if (isApplied.HasValue)
        {
            query = query.Where(r => r.IsApplied == isApplied.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.Priority)
            .ThenByDescending(r => r.CriticalVulnerabilityCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}