using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class CompliancePolicyRepository : GenericRepository<CompliancePolicy, Guid>, ICompliancePolicyRepository
{
    public CompliancePolicyRepository(NmsDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CompliancePolicy>> GetActivePoliciesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CompliancePolicy>> GetActivePoliciesForDeviceTypeAsync(
        DeviceType deviceType,
        string? vendor = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(p => p.IsActive && (!p.TargetDeviceType.HasValue || p.TargetDeviceType.Value == deviceType));

        if (!string.IsNullOrWhiteSpace(vendor))
        {
            query = query.Where(p => string.IsNullOrEmpty(p.TargetVendor) || p.TargetVendor == vendor);
        }

        return await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<CompliancePolicy> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm,
        ComplianceCategory? category,
        ComplianceSeverity? severity,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        }

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(p => p.Severity == severity.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}