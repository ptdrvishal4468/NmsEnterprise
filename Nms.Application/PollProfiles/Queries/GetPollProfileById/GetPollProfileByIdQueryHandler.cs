using MediatR;
using Nms.Application.PollProfiles.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Queries.GetPollProfileById;

public class GetPollProfileByIdQueryHandler : IRequestHandler<GetPollProfileByIdQuery, PollProfileDto?>
{
    private readonly IPollProfileRepository _repository;

    public GetPollProfileByIdQueryHandler(IPollProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<PollProfileDto?> Handle(GetPollProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile == null)
            return null;

        return new PollProfileDto(
            profile.Id,
            profile.Name,
            profile.Description,
            profile.IntervalSeconds,
            profile.TimeoutSeconds,
            profile.RetryCount,
            profile.IsDefault,
            profile.IsEnabled
        );
    }
}