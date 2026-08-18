using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Commands.AnalyzeAllDevicesVulnerabilities;

public class AnalyzeAllDevicesVulnerabilitiesCommandHandler : IRequestHandler<AnalyzeAllDevicesVulnerabilitiesCommand, FirmwareRiskSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IVulnerabilityAnalysisEngine _analysisEngine;
    private readonly IFirmwareUpgradeRecommendationEngine _recommendationEngine;

    public AnalyzeAllDevicesVulnerabilitiesCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IVulnerabilityAnalysisEngine analysisEngine,
        IFirmwareUpgradeRecommendationEngine recommendationEngine)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _analysisEngine = analysisEngine;
        _recommendationEngine = recommendationEngine;
    }

    public async Task<FirmwareRiskSummaryDto> Handle(AnalyzeAllDevicesVulnerabilitiesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var devices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var tenantDevices = devices.Where(d => d.TenantId == tenantId).ToList();

        var vulnerableDeviceCount = 0;
        var totalActiveVulns = 0;
        var criticalVulns = 0;
        var highVulns = 0;
        var mediumVulns = 0;
        var lowVulns = 0;

        foreach (var device in tenantDevices)
        {
            var matches = await _analysisEngine.AnalyzeDeviceAsync(tenantId, device, cancellationToken);
            var activeMatches = matches.Where(m => m.MatchStatus == VulnerabilityStatus.Active).ToList();

            if (activeMatches.Count > 0)
            {
                vulnerableDeviceCount++;
                totalActiveVulns += activeMatches.Count;

                foreach (var match in activeMatches)
                {
                    var vuln = match.Vulnerability ?? await _unitOfWork.Vulnerabilities.GetByIdAsync(match.VulnerabilityId, cancellationToken);
                    if (vuln != null)
                    {
                        switch (vuln.Severity)
                        {
                            case VulnerabilitySeverity.Critical: criticalVulns++; break;
                            case VulnerabilitySeverity.High: highVulns++; break;
                            case VulnerabilitySeverity.Medium: mediumVulns++; break;
                            case VulnerabilitySeverity.Low: lowVulns++; break;
                        }
                    }
                }

                await _recommendationEngine.GenerateRecommendationForDeviceAsync(tenantId, device, activeMatches, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (_, recCount) = await _unitOfWork.FirmwareUpgradeRecommendations.GetPagedAsync(
            tenantId, 1, 1, isApplied: false, cancellationToken: cancellationToken);

        return new FirmwareRiskSummaryDto(
            tenantDevices.Count,
            vulnerableDeviceCount,
            totalActiveVulns,
            criticalVulns,
            highVulns,
            mediumVulns,
            lowVulns,
            recCount);
    }
}