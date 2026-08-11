using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IDeviceHealthHistoryRepository : IGenericRepository<DeviceHealthHistory, Guid>
{
    Task<(IReadOnlyList<DeviceHealthHistory> Items, int TotalCount)> GetPagedByDeviceIdAsync(
        Guid deviceId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}