using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetSecurityAdvisoriesPaged;

public class GetSecurityAdvisoriesPagedQueryHandler : IRequestHandler<GetSecurityAdvisoriesPagedQuery, PagedResult<SecurityAdvisoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetSecurityAdvisoriesPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<SecurityAdvisoryDto>> Handle(GetSecurityAdvisoriesPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var (items, totalCount) = await _unitOfWork.SecurityAdvisories.GetPagedAsync(
            tenantId,
            request.Page,
            request.PageSize,
            request.Severity,
            request.SearchTerm,
            cancellationToken);

        var dtos = items.Select(sa => new SecurityAdvisoryDto(
            sa.Id,
            sa.TenantId,
            sa.AdvisoryId,
            sa.Vendor,
            sa.Title,
            sa.Summary,
            sa.Severity,
            sa.RemediationGuidance,
            sa.ReferenceUrl,
            sa.PublishedAtUtc,
            sa.CreatedAtUtc)).ToList();

        return new PagedResult<SecurityAdvisoryDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}