using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ThreatDetectionRuleRepository : GenericRepository<ThreatDetectionRule, Guid>, IThreatDetectionRuleRepository
{
    public ThreatDetectionRuleRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<ThreatDetectionRule?> GetRuleByThreatTypeAsync(
        Guid tenantId,
        ThreatType threatType,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ThreatType == threatType && r.IsEnabled, cancellationToken);
    }

    public async Task<(IReadOnlyList<ThreatDetectionRule> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(r => r.RuleName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}