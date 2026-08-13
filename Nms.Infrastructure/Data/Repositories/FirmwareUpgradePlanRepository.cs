using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class FirmwareUpgradePlanRepository : GenericRepository<FirmwareUpgradePlan, Guid>, IFirmwareUpgradePlanRepository
{
    public FirmwareUpgradePlanRepository(NmsDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<FirmwareUpgradePlan>> GetByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(p => p.Device)
            .Where(p => p.TenantId == tenantId && p.DeviceId == deviceId)
            .OrderByDescending(p => p.PlannedDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<FirmwareUpgradePlan?> GetActivePlanByDeviceIdAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(p => p.Device)
            .FirstOrDefaultAsync(p => p.TenantId == tenantId &&
                                      p.DeviceId == deviceId &&
                                      p.Status == Domain.Enums.UpgradePlanStatus.Planned, cancellationToken);
    }
}