using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class SiteRepository : GenericRepository<Site, Guid>, ISiteRepository
{
    public SiteRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<Site?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == normalizedCode, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeSiteId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = DbSet.AsNoTracking().Where(s => s.Code == normalizedCode);

        if (excludeSiteId.HasValue)
        {
            query = query.Where(s => s.Id != excludeSiteId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Site?> GetWithHierarchyAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Buildings)
                .ThenInclude(b => b.Floors)
                    .ThenInclude(f => f.Rooms)
                        .ThenInclude(r => r.Racks)
            .FirstOrDefaultAsync(s => s.Id == siteId, cancellationToken);
    }
}