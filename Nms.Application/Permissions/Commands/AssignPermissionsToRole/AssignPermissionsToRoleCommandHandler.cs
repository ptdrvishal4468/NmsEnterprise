using MediatR;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Permissions.Commands.AssignPermissionsToRole;

public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, bool>
{
    private readonly IGenericRepository<Role, Guid> _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPermissionsToRoleCommandHandler(
        IGenericRepository<Role, Guid> roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID '{request.RoleId}' was not found.");
        }

        // 2. Remove existing permissions for this Role
        var existingMappings = await _rolePermissionRepository.FindAsync(
            rp => rp.RoleId == request.RoleId,
            cancellationToken);

        foreach (var mapping in existingMappings)
        {
            _rolePermissionRepository.Delete(mapping);
        }

        // 3. Add new permission mappings
        if (request.PermissionIds != null && request.PermissionIds.Count > 0)
        {
            var newMappings = request.PermissionIds
                .Distinct()
                .Select(permissionId => new RolePermission(request.RoleId, permissionId));

            foreach (var mapping in newMappings)
            {
                await _rolePermissionRepository.AddAsync(mapping, cancellationToken);
            }
        }

        // 4. Commit unit of work
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}