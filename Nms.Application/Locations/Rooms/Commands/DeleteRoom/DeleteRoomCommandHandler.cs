using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(request.Id, cancellationToken);
        if (room == null)
        {
            return false;
        }

        _unitOfWork.Rooms.Remove(room);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}