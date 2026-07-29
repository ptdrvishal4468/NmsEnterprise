using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class DeviceRepository : GenericRepository<Device, Guid>, IDeviceRepository
{
    public DeviceRepository(NmsDbContext context) : base(context) { }

    public async Task<Device?> GetByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.IpAddress == ipAddress, cancellationToken);
    }

    public async Task<IReadOnlyList<Device>> GetDevicesByStatusAsync(DeviceStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(d => d.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Device>> GetActiveDevicesForPollingAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(d => d.Status != DeviceStatus.Offline).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(d => d.IpAddress == ipAddress, cancellationToken);
    }
}