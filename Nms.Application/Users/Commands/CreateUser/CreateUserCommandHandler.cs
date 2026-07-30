using Nms.Application.Common.Interfaces;
using Nms.Application.Users.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserResponseDto> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Hash raw password securely using BCrypt
        var passwordHash = _passwordHasher.HashPassword(command.Password);

        // Instantiate User entity
        var user = new User(Guid.NewGuid(), command.TenantId, command.Email, passwordHash);

        // Assign initial roles
        if (command.RoleIds != null && command.RoleIds.Any())
        {
            foreach (var roleId in command.RoleIds)
            {
                user.AssignRole(roleId);
            }
        }

        // Add to DbContext via UnitOfWork tracking and commit transaction
        // (Note: In future phases, repository extensions will expose explicit UserRepository queries)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUserResponseDto(user.Id, user.Email, user.TenantId, user.CreatedAtUtc);
    }
}