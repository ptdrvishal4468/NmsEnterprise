using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IConfigurationDriftRepository : IGenericRepository<ConfigurationDriftRecord, Guid>
{
    Task<ConfigurationDriftRecord?> GetLatestByDeviceIdAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ConfigurationDriftRecord> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? deviceId = null,
        bool? hasDrift = null,
        CancellationToken cancellationToken = default);
}