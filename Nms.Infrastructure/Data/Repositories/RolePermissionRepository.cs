using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly NmsDbContext _context;

    public RolePermissionRepository(NmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RolePermission>> FindAsync(Func<RolePermission, bool> predicate, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_context.RolePermissions.Where(predicate).ToList().AsReadOnly());
    }

    public void Delete(RolePermission entity)
    {
        _context.RolePermissions.Remove(entity);
    }

    public async Task AddAsync(RolePermission entity, CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddAsync(entity, cancellationToken);
    }
}
