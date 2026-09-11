using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Topology.Commands.DeleteTopologyLink;

public class DeleteTopologyLinkCommandHandler : IRequestHandler<DeleteTopologyLinkCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTopologyLinkCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteTopologyLinkCommand request, CancellationToken cancellationToken)
    {
        var link = await _unitOfWork.TopologyLinks.GetByIdAsync(request.Id, cancellationToken);
        if (link == null)
            return false;

        _unitOfWork.TopologyLinks.Remove(link);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}