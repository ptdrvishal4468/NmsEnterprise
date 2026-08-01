using MediatR;
using Nms.Application.Users.Dtos;

namespace Nms.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    Guid TenantId,
    string Email,
    string Password,
    List<Guid> RoleIds
) : IRequest<CreateUserResponseDto>;