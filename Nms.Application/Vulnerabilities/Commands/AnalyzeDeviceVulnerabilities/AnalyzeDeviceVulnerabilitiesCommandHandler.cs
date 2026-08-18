using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Commands.AnalyzeDeviceVulnerabilities;

public class AnalyzeDeviceVulnerabilitiesCommandHandler : IRequestHandler<AnalyzeDeviceVulnerabilitiesCommand, IReadOnlyList<DeviceVulnerabilityMatchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IVulnerabilityAnalysisEngine _analysisEngine;
    private readonly IFirmwareUpgradeRecommendationEngine _recommendationEngine;

    public AnalyzeDeviceVulnerabilitiesCommandHandler(
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

    public async Task<IReadOnlyList<DeviceVulnerabilityMatchDto>> Handle(AnalyzeDeviceVulnerabilitiesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null || device.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var matches = await _analysisEngine.AnalyzeDeviceAsync(tenantId, device, cancellationToken);
        var activeMatches = matches.Where(m => m.MatchStatus == VulnerabilityStatus.Active).ToList();

        if (activeMatches.Count > 0)
        {
            await _recommendationEngine.GenerateRecommendationForDeviceAsync(tenantId, device, activeMatches, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var results = await _unitOfWork.DeviceVulnerabilityMatches.GetByDeviceIdAsync(tenantId, device.Id, cancellationToken);

        return results.Select(m => new DeviceVulnerabilityMatchDto(
            m.Id,
            m.TenantId,
            m.DeviceId,
            device.Name,
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
    }
}