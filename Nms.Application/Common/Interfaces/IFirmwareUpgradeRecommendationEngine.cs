using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IFirmwareUpgradeRecommendationEngine
{
    Task<FirmwareUpgradeRecommendation?> GenerateRecommendationForDeviceAsync(
        Guid tenantId,
        Device device,
        IReadOnlyList<DeviceVulnerabilityMatch> activeMatches,
        CancellationToken cancellationToken = default);
}