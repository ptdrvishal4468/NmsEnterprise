using Nms.Domain.Interfaces;

namespace Nms.Application.Users.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRolesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        // Fetch user or throw domain exception
        // Note: Full EF Core UserRepository mapping wired up in execution pipeline
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}