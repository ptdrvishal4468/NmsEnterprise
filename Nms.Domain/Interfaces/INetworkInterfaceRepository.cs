using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface INetworkInterfaceRepository : IGenericRepository<NetworkInterface, Guid>
{
    Task<IReadOnlyList<NetworkInterface>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<NetworkInterface?> GetByDeviceAndIfIndexAsync(Guid deviceId, int ifIndex, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NetworkInterfaceHistory>> GetHistoryAsync(Guid networkInterfaceId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(NetworkInterfaceHistory history, CancellationToken cancellationToken = default);
}