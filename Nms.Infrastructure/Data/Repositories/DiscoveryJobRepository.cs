using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Org.BouncyCastle.Asn1;

namespace Nms.Infrastructure.Data.Repositories;

public class DiscoveryJobRepository : GenericRepository<DiscoveryJob, Guid>, IDiscoveryJobRepository
{
    public DiscoveryJobRepository(NmsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DiscoveryJob?> GetWithCandidatesAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(j => j.Candidates)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscoveryJob>> GetActiveJobsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(j => j.Status == DiscoveryJobStatus.Running || j.Status == DiscoveryJobStatus.Pending)
            .ToListAsync(cancellationToken);
    }
}