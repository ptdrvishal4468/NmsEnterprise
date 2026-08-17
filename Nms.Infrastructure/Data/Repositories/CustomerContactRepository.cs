using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class CustomerContactRepository : GenericRepository<CustomerContact, Guid>, ICustomerContactRepository
{
    public CustomerContactRepository(NmsDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CustomerContact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(cc => cc.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerContact?> GetPrimaryContactAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(cc => cc.CustomerId == customerId && cc.IsPrimary, cancellationToken);
    }

    public async Task<bool> EmailExistsInCustomerAsync(Guid customerId, string email, Guid? excludeContactId = null, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .AnyAsync(cc => cc.CustomerId == customerId && cc.Email == email && (!excludeContactId.HasValue || cc.Id != excludeContactId.Value), cancellationToken);
    }
}