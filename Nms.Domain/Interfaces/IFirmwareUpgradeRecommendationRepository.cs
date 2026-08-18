using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IFirmwareUpgradeRecommendationRepository : IGenericRepository<FirmwareUpgradeRecommendation, Guid>
{
    Task<IReadOnlyList<FirmwareUpgradeRecommendation>> GetActiveByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<FirmwareUpgradeRecommendation> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? deviceId = null,
        RecommendationPriority? priority = null,
        bool? isApplied = null,
        CancellationToken cancellationToken = default);
}