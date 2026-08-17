using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Assets.Commands.DeleteAsset;

public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _unitOfWork.Assets.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
        {
            return false;
        }

        _unitOfWork.Assets.Remove(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}