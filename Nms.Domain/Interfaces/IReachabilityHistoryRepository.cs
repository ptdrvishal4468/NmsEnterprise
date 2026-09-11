using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IReachabilityHistoryRepository : IGenericRepository<DeviceReachabilityHistory, Guid>
{
    Task<IEnumerable<DeviceReachabilityHistory>> GetByDeviceIdAsync(Guid deviceId, int take, CancellationToken cancellationToken = default);
    Task<DeviceReachabilityHistory?> GetLatestByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<DeviceReachabilityHistory> Items, int TotalCount)> GetPagedByDeviceIdAsync(
        Guid deviceId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}