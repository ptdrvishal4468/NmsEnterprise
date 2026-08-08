using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.PollProfiles.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Queries.GetPollProfilesPaged;

public class GetPollProfilesPagedQueryHandler : IRequestHandler<GetPollProfilesPagedQuery, PagedResult<PollProfileDto>>
{
    private readonly IPollProfileRepository _repository;

    public GetPollProfilesPagedQueryHandler(IPollProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PollProfileDto>> Handle(GetPollProfilesPagedQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _repository.GetAllAsync(cancellationToken);

        var dtos = profiles
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PollProfileDto(
                p.Id,
                p.Name,
                p.Description,
                p.IntervalSeconds,
                p.TimeoutSeconds,
                p.RetryCount,
                p.IsDefault,
                p.IsEnabled))
            .ToList();

        return new PagedResult<PollProfileDto>(dtos, profiles.Count(), request.PageNumber, request.PageSize);
    }
}