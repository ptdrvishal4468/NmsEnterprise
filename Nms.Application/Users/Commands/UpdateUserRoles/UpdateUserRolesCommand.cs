namespace Nms.Application.Users.Commands.UpdateUserRoles;

public record UpdateUserRolesCommand(
    Guid UserId,
    List<Guid> RoleIds
);