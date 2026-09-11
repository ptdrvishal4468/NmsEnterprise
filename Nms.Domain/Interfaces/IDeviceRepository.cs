using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

/// <summary>
/// Dedicated repository contract for complex device aggregate queries.
/// </summary>
public interface IDeviceRepository : IGenericRepository<Device, Guid>
{
    Task<Device?> GetByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task<Device?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Device>> GetDevicesByStatusAsync(DeviceStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Device>> GetActiveDevicesForPollingAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIpAddressExcludingIdAsync(string ipAddress, Guid excludeDeviceId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Device> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm,
        DeviceType? deviceType,
        DeviceStatus? status,
        CancellationToken cancellationToken = default);
}