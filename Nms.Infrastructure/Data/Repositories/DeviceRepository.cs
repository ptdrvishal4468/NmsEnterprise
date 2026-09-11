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
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IpAddress == ipAddress.Trim(), cancellationToken);
    }

    public async Task<Device?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber.Trim(), cancellationToken);
    }

    public async Task<IReadOnlyList<Device>> GetDevicesByStatusAsync(DeviceStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(d => d.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Device>> GetActiveDevicesForPollingAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(d => d.Status != DeviceStatus.Offline)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(d => d.IpAddress == ipAddress.Trim(), cancellationToken);
    }

    public async Task<bool> ExistsByIpAddressExcludingIdAsync(string ipAddress, Guid excludeDeviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(d => d.IpAddress == ipAddress.Trim() && d.Id != excludeDeviceId, cancellationToken);
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
            var term = searchTerm.Trim();
            // Sargable LIKE query using column collation
            query = query.Where(d =>
                EF.Functions.Like(d.Name, $"%{term}%") ||
                EF.Functions.Like(d.IpAddress, $"%{term}%") ||
                (d.Hostname != null && EF.Functions.Like(d.Hostname, $"%{term}%")) ||
                (d.Vendor != null && EF.Functions.Like(d.Vendor, $"%{term}%")) ||
                (d.Model != null && EF.Functions.Like(d.Model, $"%{term}%")) ||
                (d.SerialNumber != null && EF.Functions.Like(d.SerialNumber, $"%{term}%")));
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