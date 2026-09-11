using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetFirmwareRiskSummary;

public class GetFirmwareRiskSummaryQueryHandler : IRequestHandler<GetFirmwareRiskSummaryQuery, FirmwareRiskSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetFirmwareRiskSummaryQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<FirmwareRiskSummaryDto> Handle(GetFirmwareRiskSummaryQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var devices = (await _unitOfWork.Devices.GetAllAsync(cancellationToken)).Where(d => d.TenantId == tenantId).ToList();

        var (matches, _) = await _unitOfWork.DeviceVulnerabilityMatches.GetPagedAsync(
            tenantId, 1, int.MaxValue, status: VulnerabilityStatus.Active, cancellationToken: cancellationToken);

        var vulnerableDeviceIds = matches.Select(m => m.DeviceId).Distinct().ToHashSet();

        var critical = matches.Count(m => m.Vulnerability?.Severity == VulnerabilitySeverity.Critical);
        var high = matches.Count(m => m.Vulnerability?.Severity == VulnerabilitySeverity.High);
        var medium = matches.Count(m => m.Vulnerability?.Severity == VulnerabilitySeverity.Medium);
        var low = matches.Count(m => m.Vulnerability?.Severity == VulnerabilitySeverity.Low);

        var (_, pendingRecs) = await _unitOfWork.FirmwareUpgradeRecommendations.GetPagedAsync(
            tenantId, 1, 1, isApplied: false, cancellationToken: cancellationToken);

        return new FirmwareRiskSummaryDto(
            devices.Count,
            vulnerableDeviceIds.Count,
            matches.Count,
            critical,
            high,
            medium,
            low,
            pendingRecs);
    }
}