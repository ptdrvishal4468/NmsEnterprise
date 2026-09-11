using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

/// <summary>
/// Repository interface for physical asset management queries and persistence.
/// </summary>
public interface IAssetRepository : IGenericRepository<Asset, Guid>
{
    Task<Asset?> GetByAssetTagAsync(string assetTag, CancellationToken cancellationToken = default);
    Task<Asset?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<bool> AssetTagExistsAsync(string assetTag, Guid? excludeAssetId = null, CancellationToken cancellationToken = default);
}