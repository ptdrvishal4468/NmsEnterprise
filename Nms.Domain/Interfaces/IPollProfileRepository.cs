using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IPollProfileRepository : IGenericRepository<PollProfile, Guid>
{
    Task<PollProfile?> GetDefaultProfileAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollProfile>> GetEnabledProfilesAsync(Guid tenantId, CancellationToken cancellationToken = default);
}