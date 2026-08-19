using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Queries.GetAlertRulesPaged;

public class GetAlertRulesPagedQueryHandler : IRequestHandler<GetAlertRulesPagedQuery, PagedResult<AlertRuleDto>>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly ICacheService? _cacheService;

    public GetAlertRulesPagedQueryHandler(
        IAlertRuleRepository ruleRepository,
        ICacheService? cacheService = null)
    {
        _ruleRepository = ruleRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<AlertRuleDto>> Handle(GetAlertRulesPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);
        var cacheKey = $"rules:alerts:paged:{pageIndex}:{pageSize}:{request.DeviceId?.ToString() ?? "all"}";

        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<PagedResult<AlertRuleDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        var (items, totalCount) = await _ruleRepository.GetPagedAsync(
            pageIndex,
            pageSize,
            request.DeviceId,
            cancellationToken);

        var dtos = items.Select(r => new AlertRuleDto(
            r.Id,
            r.TenantId,
            r.Name,
            r.Description,
            r.MetricType,
            r.Operator,
            r.ThresholdValue,
            r.Severity,
            r.IsEnabled,
            r.DeviceId,
            r.CreatedAtUtc)).ToList();

        var result = new PagedResult<AlertRuleDto>(dtos, totalCount, pageIndex, pageSize);

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);
        }

        return result;
    }
}