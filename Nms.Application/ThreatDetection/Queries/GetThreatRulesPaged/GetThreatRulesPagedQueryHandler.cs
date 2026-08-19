using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Queries.GetThreatRulesPaged;

public class GetThreatRulesPagedQueryHandler : IRequestHandler<GetThreatRulesPagedQuery, PagedResult<ThreatDetectionRuleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public GetThreatRulesPagedQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ICacheService? cacheService = null)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<ThreatDetectionRuleDto>> Handle(GetThreatRulesPagedQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"rules:threats:paged:{request.PageNumber}:{request.PageSize}";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<PagedResult<ThreatDetectionRuleDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        var (items, totalCount) = await _unitOfWork.ThreatDetectionRules.GetPagedAsync(
            _tenantContext.TenantId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(r => new ThreatDetectionRuleDto
        {
            Id = r.Id,
            TenantId = r.TenantId,
            RuleName = r.RuleName,
            ThreatType = r.ThreatType,
            DefaultSeverity = r.DefaultSeverity,
            FailureThreshold = r.FailureThreshold,
            TimeWindowMinutes = r.TimeWindowMinutes,
            IsEnabled = r.IsEnabled,
            Description = r.Description
        }).ToList();

        var result = new PagedResult<ThreatDetectionRuleDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);
        }

        return result;
    }
}