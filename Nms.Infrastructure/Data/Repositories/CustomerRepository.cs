using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class CustomerRepository : GenericRepository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(NmsDbContext context) : base(context) { }

    public async Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .AnyAsync(c => c.Code == code && (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value), cancellationToken);
    }

    public async Task<Customer?> GetWithHierarchyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(c => c.ParentCustomer)
            .Include(c => c.ChildCustomers)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetWithContactsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetChildrenAsync(Guid parentCustomerId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(c => c.ParentCustomerId == parentCustomerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasCircularHierarchyAsync(Guid customerId, Guid prospectiveParentId, CancellationToken cancellationToken = default)
    {
        if (customerId == prospectiveParentId)
        {
            return true;
        }

        var currentParentId = (Guid?)prospectiveParentId;
        var visited = new HashSet<Guid> { customerId, prospectiveParentId };

        while (currentParentId.HasValue)
        {
            var parent = await DbSet.AsNoTracking()
                .Where(c => c.Id == currentParentId.Value)
                .Select(c => new { c.ParentCustomerId })
                .FirstOrDefaultAsync(cancellationToken);

            if (parent == null || !parent.ParentCustomerId.HasValue)
            {
                return false;
            }

            if (parent.ParentCustomerId.Value == customerId)
            {
                return true;
            }

            if (!visited.Add(parent.ParentCustomerId.Value))
            {
                return true;
            }

            currentParentId = parent.ParentCustomerId.Value;
        }

        return false;
    }
}