using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Commands.DeleteFloor;

public class DeleteFloorCommandHandler : IRequestHandler<DeleteFloorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFloorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteFloorCommand request, CancellationToken cancellationToken)
    {
        var floor = await _unitOfWork.Floors.GetByIdAsync(request.Id, cancellationToken);
        if (floor == null)
        {
            return false;
        }

        _unitOfWork.Floors.Remove(floor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}