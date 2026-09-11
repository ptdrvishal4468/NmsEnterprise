using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Sites.Commands.DeleteSite;

public class DeleteSiteCommandHandler : IRequestHandler<DeleteSiteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSiteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _unitOfWork.Sites.GetByIdAsync(request.Id, cancellationToken);
        if (site == null)
        {
            return false;
        }

        _unitOfWork.Sites.Remove(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}