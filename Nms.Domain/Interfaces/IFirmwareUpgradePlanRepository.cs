using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IFirmwareUpgradePlanRepository : IGenericRepository<FirmwareUpgradePlan, Guid>
{
    Task<IReadOnlyList<FirmwareUpgradePlan>> GetByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default);
    Task<FirmwareUpgradePlan?> GetActivePlanByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default);
}