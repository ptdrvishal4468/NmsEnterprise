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

    public GetThreatRulesPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<ThreatDetectionRuleDto>> Handle(GetThreatRulesPagedQuery request, CancellationToken cancellationToken)
    {
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

        return new PagedResult<ThreatDetectionRuleDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}