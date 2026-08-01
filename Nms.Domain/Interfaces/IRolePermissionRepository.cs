using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<RolePermission>> FindAsync(Func<RolePermission, bool> predicate, CancellationToken cancellationToken = default);
    void Delete(RolePermission entity);
    Task AddAsync(RolePermission entity, CancellationToken cancellationToken = default);
}
