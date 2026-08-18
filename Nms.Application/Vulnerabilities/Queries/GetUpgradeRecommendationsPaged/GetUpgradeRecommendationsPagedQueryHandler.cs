using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetUpgradeRecommendationsPaged;

public class GetUpgradeRecommendationsPagedQueryHandler : IRequestHandler<GetUpgradeRecommendationsPagedQuery, PagedResult<FirmwareUpgradeRecommendationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetUpgradeRecommendationsPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<FirmwareUpgradeRecommendationDto>> Handle(GetUpgradeRecommendationsPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var (items, totalCount) = await _unitOfWork.FirmwareUpgradeRecommendations.GetPagedAsync(
            tenantId,
            request.Page,
            request.PageSize,
            request.DeviceId,
            request.Priority,
            request.IsApplied,
            cancellationToken);

        var dtos = items.Select(r => new FirmwareUpgradeRecommendationDto(
            r.Id,
            r.TenantId,
            r.DeviceId,
            r.Device?.Name,
            r.CurrentVersion,
            r.RecommendedVersion,
            r.Priority,
            r.Reasoning,
            r.AssociatedVulnerabilityCount,
            r.CriticalVulnerabilityCount,
            r.IsApplied,
            r.CreatedAtUtc)).ToList();

        return new PagedResult<FirmwareUpgradeRecommendationDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}