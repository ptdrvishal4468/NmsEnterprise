using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Commands.DeleteBuilding;

public class DeleteBuildingCommandHandler : IRequestHandler<DeleteBuildingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBuildingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await _unitOfWork.Buildings.GetByIdAsync(request.Id, cancellationToken);
        if (building == null)
        {
            return false;
        }

        _unitOfWork.Buildings.Remove(building);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}