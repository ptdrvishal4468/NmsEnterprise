using MediatR;

namespace Nms.Application.Permissions.Commands.AssignPermissionsToRole;

public record AssignPermissionsToRoleCommand(
    Guid RoleId,
    List<int> PermissionIds
) : IRequest<bool>;