using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IDeviceMetricRepository : IGenericRepository<DeviceMetricRaw, long>
{
    Task AddBulkAsync(IEnumerable<DeviceMetricRaw> metrics, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeviceMetricRaw>> GetMetricsForDeviceAsync(
        Guid deviceId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);
}