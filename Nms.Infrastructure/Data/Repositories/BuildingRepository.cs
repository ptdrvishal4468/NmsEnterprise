using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class BuildingRepository : GenericRepository<Building, Guid>, IBuildingRepository
{
    public BuildingRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Building>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(b => b.Floors)
            .Where(b => b.SiteId == siteId)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsInSiteAsync(Guid siteId, string code, Guid? excludeBuildingId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = DbSet.AsNoTracking().Where(b => b.SiteId == siteId && b.Code == normalizedCode);

        if (excludeBuildingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBuildingId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}