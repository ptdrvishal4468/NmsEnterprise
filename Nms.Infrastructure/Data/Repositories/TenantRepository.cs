using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class TenantRepository : GenericRepository<Tenant, Guid>, ITenantRepository
{
    public TenantRepository(NmsDbContext Context) : base(Context)
    {
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Tenants.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Context.Tenants
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}