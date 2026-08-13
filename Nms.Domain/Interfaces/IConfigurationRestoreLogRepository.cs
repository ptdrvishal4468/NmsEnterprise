using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IConfigurationRestoreLogRepository : IGenericRepository<ConfigurationRestoreLog, Guid>
{
    Task<IReadOnlyList<ConfigurationRestoreLog>> GetByDeviceIdAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveRestoreInProgressAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default);
}