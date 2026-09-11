using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetVulnerabilitiesPaged;

public class GetVulnerabilitiesPagedQueryHandler : IRequestHandler<GetVulnerabilitiesPagedQuery, PagedResult<VulnerabilityDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetVulnerabilitiesPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<VulnerabilityDto>> Handle(GetVulnerabilitiesPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var (items, totalCount) = await _unitOfWork.Vulnerabilities.GetPagedAsync(
            tenantId,
            request.Page,
            request.PageSize,
            request.Severity,
            request.SearchTerm,
            cancellationToken);

        var dtos = items.Select(v => new VulnerabilityDto(
            v.Id,
            v.TenantId,
            v.CveId,
            v.Title,
            v.Description,
            v.Severity,
            v.CvssScore,
            v.AffectedVendor,
            v.AffectedModel,
            v.AffectedVersionMin,
            v.AffectedVersionMax,
            v.PatchedVersion,
            v.PublishedAtUtc,
            v.CreatedAtUtc)).ToList();

        return new PagedResult<VulnerabilityDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}