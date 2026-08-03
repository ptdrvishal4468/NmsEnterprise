using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface ITenantRepository : IGenericRepository<Tenant, Guid>
{
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tenant>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}