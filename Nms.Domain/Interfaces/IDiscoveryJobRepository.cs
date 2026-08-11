using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IDiscoveryJobRepository : IGenericRepository<DiscoveryJob, Guid>
{
    Task<DiscoveryJob?> GetWithCandidatesAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DiscoveryJob>> GetActiveJobsAsync(CancellationToken cancellationToken = default);
}