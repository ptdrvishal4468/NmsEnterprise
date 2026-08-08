using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class PollProfileRepository : GenericRepository<PollProfile, Guid>, IPollProfileRepository
{
    public PollProfileRepository(NmsDbContext context) : base(context) { }

    public async Task<PollProfile?> GetDefaultProfileAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.IsDefault && p.IsEnabled, cancellationToken);
    }

    public async Task<IEnumerable<PollProfile>> GetEnabledProfilesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.TenantId == tenantId && p.IsEnabled)
            .ToListAsync(cancellationToken);
    }
}