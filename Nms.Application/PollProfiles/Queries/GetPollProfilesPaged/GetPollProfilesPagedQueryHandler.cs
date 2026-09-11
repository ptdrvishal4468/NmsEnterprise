using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.PollProfiles.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Queries.GetPollProfilesPaged;

public class GetPollProfilesPagedQueryHandler : IRequestHandler<GetPollProfilesPagedQuery, PagedResult<PollProfileDto>>
{
    private readonly IPollProfileRepository _repository;
    private readonly ICacheService? _cacheService;

    public GetPollProfilesPagedQueryHandler(
        IPollProfileRepository repository,
        ICacheService? cacheService = null)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<PollProfileDto>> Handle(GetPollProfilesPagedQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"rules:poll-profiles:paged:{request.PageNumber}:{request.PageSize}";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<PagedResult<PollProfileDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

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

        var result = new PagedResult<PollProfileDto>(dtos, profiles.Count, request.PageNumber, request.PageSize);

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);
        }

        return result;
    }
}