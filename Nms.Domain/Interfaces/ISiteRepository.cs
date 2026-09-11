namespace Nms.Domain.Interfaces;

using Nms.Domain.Entities;

public interface ISiteRepository : IGenericRepository<Site, Guid>
{
    Task<Site?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeSiteId = null, CancellationToken cancellationToken = default);
    Task<Site?> GetWithHierarchyAsync(Guid siteId, CancellationToken cancellationToken = default);
}