using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class NetworkInterfaceRepository : GenericRepository<NetworkInterface, Guid>, INetworkInterfaceRepository
{
    public NetworkInterfaceRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<NetworkInterface>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await Context.NetworkInterfaces
            .Where(ni => ni.DeviceId == deviceId)
            .OrderBy(ni => ni.IfIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<NetworkInterface?> GetByDeviceAndIfIndexAsync(Guid deviceId, int ifIndex, CancellationToken cancellationToken = default)
    {
        return await Context.NetworkInterfaces
            .FirstOrDefaultAsync(ni => ni.DeviceId == deviceId && ni.IfIndex == ifIndex, cancellationToken);
    }

    public async Task<IReadOnlyList<NetworkInterfaceHistory>> GetHistoryAsync(
        Guid networkInterfaceId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken = default)
    {
        return await Context.NetworkInterfaceHistories
            .Where(nih => nih.NetworkInterfaceId == networkInterfaceId && nih.TimestampUtc >= startUtc && nih.TimestampUtc <= endUtc)
            .OrderByDescending(nih => nih.TimestampUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(NetworkInterfaceHistory history, CancellationToken cancellationToken = default)
    {
        await Context.NetworkInterfaceHistories.AddAsync(history, cancellationToken);
    }
}