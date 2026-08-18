using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Services;

public class FirmwareUpgradeRecommendationEngine : IFirmwareUpgradeRecommendationEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFirmwareVersionComparator _versionComparator;

    public FirmwareUpgradeRecommendationEngine(
        IUnitOfWork unitOfWork,
        IFirmwareVersionComparator versionComparator)
    {
        _unitOfWork = unitOfWork;
        _versionComparator = versionComparator;
    }

    public async Task<FirmwareUpgradeRecommendation?> GenerateRecommendationForDeviceAsync(
        Guid tenantId,
        Device device,
        IReadOnlyList<DeviceVulnerabilityMatch> activeMatches,
        CancellationToken cancellationToken = default)
    {
        if (activeMatches.Count == 0 || string.IsNullOrWhiteSpace(device.FirmwareVersion))
            return null;

        var criticalCount = 0;
        var highCount = 0;
        string? candidatePatchedVersion = null;

        foreach (var match in activeMatches)
        {
            var vuln = match.Vulnerability ?? await _unitOfWork.Vulnerabilities.GetByIdAsync(match.VulnerabilityId, cancellationToken);
            if (vuln == null) continue;

            if (vuln.Severity == VulnerabilitySeverity.Critical) criticalCount++;
            else if (vuln.Severity == VulnerabilitySeverity.High) highCount++;

            if (!string.IsNullOrWhiteSpace(vuln.PatchedVersion))
            {
                if (candidatePatchedVersion == null || _versionComparator.Compare(vuln.PatchedVersion, candidatePatchedVersion) > 0)
                {
                    candidatePatchedVersion = vuln.PatchedVersion;
                }
            }
        }

        // Check if baseline exists
        string recommendedVersion;
        string reasoning;

        if (!string.IsNullOrWhiteSpace(device.Vendor) && !string.IsNullOrWhiteSpace(device.Model))
        {
            var baseline = await _unitOfWork.FirmwareBaselines.GetByVendorAndModelAsync(tenantId, device.Vendor, device.Model, cancellationToken);
            if (baseline != null && _versionComparator.Compare(baseline.TargetVersion, device.FirmwareVersion) > 0)
            {
                recommendedVersion = baseline.TargetVersion;
                reasoning = $"Target version {recommendedVersion} aligns with tenant firmware baseline and addresses known CVEs.";
            }
            else if (!string.IsNullOrWhiteSpace(candidatePatchedVersion))
            {
                recommendedVersion = candidatePatchedVersion;
                reasoning = $"Recommended version {recommendedVersion} patches known active CVEs (Critical: {criticalCount}, High: {highCount}).";
            }
            else
            {
                recommendedVersion = "Vendor Advisory Review Required";
                reasoning = $"Device has {activeMatches.Count} active vulnerabilities but no explicit patch version is indexed.";
            }
        }
        else if (!string.IsNullOrWhiteSpace(candidatePatchedVersion))
        {
            recommendedVersion = candidatePatchedVersion;
            reasoning = $"Recommended version {recommendedVersion} patches known active CVEs.";
        }
        else
        {
            recommendedVersion = "Vendor Advisory Review Required";
            reasoning = "Device has active vulnerabilities; consult vendor advisory for remediation.";
        }

        var priority = RecommendationPriority.Low;
        if (criticalCount > 0) priority = RecommendationPriority.Critical;
        else if (highCount > 0) priority = RecommendationPriority.High;
        else if (activeMatches.Count > 0) priority = RecommendationPriority.Medium;

        var existingRecs = await _unitOfWork.FirmwareUpgradeRecommendations.GetActiveByDeviceIdAsync(tenantId, device.Id, cancellationToken);
        var existing = existingRecs.FirstOrDefault();

        if (existing != null)
        {
            return existing;
        }

        var recommendation = new FirmwareUpgradeRecommendation(
            Guid.NewGuid(),
            tenantId,
            device.Id,
            device.FirmwareVersion,
            recommendedVersion,
            priority,
            reasoning,
            activeMatches.Count,
            criticalCount);

        await _unitOfWork.FirmwareUpgradeRecommendations.AddAsync(recommendation, cancellationToken);
        return recommendation;
    }
}