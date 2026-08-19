using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Commands.DeletePollProfile;

public class DeletePollProfileCommandHandler : IRequestHandler<DeletePollProfileCommand, bool>
{
    private readonly IPollProfileRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService? _cacheService;

    public DeletePollProfileCommandHandler(
        IPollProfileRepository repository,
        IUnitOfWork unitOfWork,
        ICacheService? cacheService = null)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeletePollProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile == null)
            return false;

        _repository.Remove(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:poll-profiles", cancellationToken);
        }

        return true;
    }
}