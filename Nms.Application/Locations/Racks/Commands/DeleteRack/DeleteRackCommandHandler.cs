using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Commands.DeleteRack;

public class DeleteRackCommandHandler : IRequestHandler<DeleteRackCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRackCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRackCommand request, CancellationToken cancellationToken)
    {
        var rack = await _unitOfWork.Racks.GetByIdAsync(request.Id, cancellationToken);
        if (rack == null)
        {
            return false;
        }

        _unitOfWork.Racks.Remove(rack);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}