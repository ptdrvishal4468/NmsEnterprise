using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class SecurityAdvisoryRepository : GenericRepository<SecurityAdvisory, Guid>, ISecurityAdvisoryRepository
{
    public SecurityAdvisoryRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<SecurityAdvisory?> GetByAdvisoryIdAsync(Guid tenantId, string advisoryId, CancellationToken cancellationToken = default)
    {
        var normalizedId = advisoryId.Trim().ToUpperInvariant();
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(sa => sa.TenantId == tenantId && sa.AdvisoryId == normalizedId, cancellationToken);
    }

    public async Task<IReadOnlyList<SecurityAdvisory>> GetByVendorAsync(Guid tenantId, string vendor, CancellationToken cancellationToken = default)
    {
        var vTrimmed = vendor.Trim();
        return await DbSet
            .AsNoTracking()
            .Where(sa => sa.TenantId == tenantId && sa.Vendor == vTrimmed)
            .OrderByDescending(sa => sa.PublishedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<SecurityAdvisory> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        VulnerabilitySeverity? severity = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(sa => sa.TenantId == tenantId);

        if (severity.HasValue)
        {
            query = query.Where(sa => sa.Severity == severity.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            query = query.Where(sa => sa.AdvisoryId.Contains(search) || sa.Title.Contains(search) || sa.Vendor.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(sa => sa.Severity)
            .ThenByDescending(sa => sa.PublishedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}