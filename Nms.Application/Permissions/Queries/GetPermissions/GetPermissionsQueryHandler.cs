using MediatR;
using Microsoft.EntityFrameworkCore;
using Nms.Application.Permissions.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    private readonly IGenericRepository<Permission, int> _permissionRepository;

    public GetPermissionsQueryHandler(IGenericRepository<Permission, int> permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IReadOnlyList<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        return permissions
            .Select(p => new PermissionDto(p.Id, p.PermissionKey, p.Description))
            .ToList();
    }
}