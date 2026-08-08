using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Commands.DeletePollProfile;

public class DeletePollProfileCommandHandler : IRequestHandler<DeletePollProfileCommand, bool>
{
    private readonly IPollProfileRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePollProfileCommandHandler(IPollProfileRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePollProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile == null)
            return false;

        _repository.Remove(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}