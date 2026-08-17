using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class AssetRepository : GenericRepository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<Asset?> GetByAssetTagAsync(string assetTag, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetTag == assetTag.Trim(), cancellationToken);
    }

    public async Task<Asset?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.DeviceId == deviceId, cancellationToken);
    }

    public async Task<bool> AssetTagExistsAsync(string assetTag, Guid? excludeAssetId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(a => a.AssetTag == assetTag.Trim());
        if (excludeAssetId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAssetId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}