namespace Nms.Domain.Interfaces;

using Nms.Domain.Entities;

public interface IBuildingRepository : IGenericRepository<Building, Guid>
{
    Task<IReadOnlyList<Building>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsInSiteAsync(Guid siteId, string code, Guid? excludeBuildingId = null, CancellationToken cancellationToken = default);
}