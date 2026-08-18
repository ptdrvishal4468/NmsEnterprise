using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetDeviceVulnerabilitiesPaged;

public class GetDeviceVulnerabilitiesPagedQueryHandler : IRequestHandler<GetDeviceVulnerabilitiesPagedQuery, PagedResult<DeviceVulnerabilityMatchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetDeviceVulnerabilitiesPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<DeviceVulnerabilityMatchDto>> Handle(GetDeviceVulnerabilitiesPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var (items, totalCount) = await _unitOfWork.DeviceVulnerabilityMatches.GetPagedAsync(
            tenantId,
            request.Page,
            request.PageSize,
            request.DeviceId,
            request.Status,
            request.Severity,
            cancellationToken);

        var dtos = items.Select(m => new DeviceVulnerabilityMatchDto(
            m.Id,
            m.TenantId,
            m.DeviceId,
            m.Device?.Name,
            m.VulnerabilityId,
            m.Vulnerability?.CveId,
            m.Vulnerability?.Title,
            m.Vulnerability?.Severity,
            m.Vulnerability?.CvssScore,
            m.SecurityAdvisoryId,
            m.SecurityAdvisory?.AdvisoryId,
            m.DetectedFirmwareVersion,
            m.MatchStatus,
            m.DetectedAtUtc,
            m.ResolutionNotes)).ToList();

        return new PagedResult<DeviceVulnerabilityMatchDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}