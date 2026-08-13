using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class FirmwareBaselineRepository : GenericRepository<FirmwareBaseline, Guid>, IFirmwareBaselineRepository
{
    public FirmwareBaselineRepository(NmsDbContext dbContext) : base(dbContext) { }

    public async Task<FirmwareBaseline?> GetByVendorAndModelAsync(Guid tenantId, string vendor, string model, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b =>
            b.TenantId == tenantId &&
            b.Vendor.ToLower() == vendor.Trim().ToLower() &&
            b.Model.ToLower() == model.Trim().ToLower(), cancellationToken);
    }

    public async Task<IReadOnlyList<FirmwareBaseline>> GetActiveBaselinesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(b => b.TenantId == tenantId && b.IsActive)
            .ToListAsync(cancellationToken);
    }
}