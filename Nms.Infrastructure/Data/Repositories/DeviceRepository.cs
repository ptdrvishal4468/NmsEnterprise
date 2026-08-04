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

    public async Task<Device?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.SerialNumber == serialNumber, cancellationToken);
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

    public async Task<bool> ExistsByIpAddressExcludingIdAsync(string ipAddress, Guid excludeDeviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(d => d.IpAddress == ipAddress && d.Id != excludeDeviceId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Device> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm,
        DeviceType? deviceType,
        DeviceStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Device> query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(term) ||
                d.IpAddress.Contains(term) ||
                (d.Hostname != null && d.Hostname.ToLower().Contains(term)) ||
                (d.Vendor != null && d.Vendor.ToLower().Contains(term)) ||
                (d.Model != null && d.Model.ToLower().Contains(term)) ||
                (d.SerialNumber != null && d.SerialNumber.ToLower().Contains(term)));
        }

        if (deviceType.HasValue)
        {
            query = query.Where(d => d.DeviceType == deviceType.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}