using MediatR;
using Nms.Application.Permissions.Dtos;

namespace Nms.Application.Permissions.Queries.GetPermissions;

public record GetPermissionsQuery() : IRequest<IReadOnlyList<PermissionDto>>;