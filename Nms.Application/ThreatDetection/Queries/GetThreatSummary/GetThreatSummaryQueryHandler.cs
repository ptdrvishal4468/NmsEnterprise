using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Queries.GetThreatSummary;

public class GetThreatSummaryQueryHandler : IRequestHandler<GetThreatSummaryQuery, ThreatSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetThreatSummaryQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ThreatSummaryDto> Handle(GetThreatSummaryQuery request, CancellationToken cancellationToken)
    {
        var (activeIndicators, _) = await _unitOfWork.ThreatIndicators.GetPagedAsync(
            _tenantContext.TenantId,
            page: 1,
            pageSize: 1000,
            threatType: null,
            severity: null,
            status: ThreatStatus.Active,
            deviceId: null,
            cancellationToken: cancellationToken);

        var (drifts, _) = await _unitOfWork.ConfigurationDrifts.GetPagedAsync(
            _tenantContext.TenantId,
            page: 1,
            pageSize: 1000,
            deviceId: null,
            hasDrift: true,
            cancellationToken: cancellationToken);

        return new ThreatSummaryDto
        {
            TotalActiveThreats = activeIndicators.Count,
            CriticalThreats = activeIndicators.Count(i => i.Severity == ThreatSeverity.Critical),
            HighThreats = activeIndicators.Count(i => i.Severity == ThreatSeverity.High),
            MediumThreats = activeIndicators.Count(i => i.Severity == ThreatSeverity.Medium),
            LowThreats = activeIndicators.Count(i => i.Severity == ThreatSeverity.Low),
            DevicesWithConfigDrift = drifts.Select(d => d.DeviceId).Distinct().Count(),
            FailedLoginThreats = activeIndicators.Count(i => i.ThreatType == ThreatType.FailedLogin),
            PortScanThreats = activeIndicators.Count(i => i.ThreatType == ThreatType.PortScanIndicator),
            UnauthorizedAccessThreats = activeIndicators.Count(i => i.ThreatType == ThreatType.UnauthorizedAccess)
        };
    }
}