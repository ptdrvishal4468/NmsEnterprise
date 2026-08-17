using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class FloorRepository : GenericRepository<Floor, Guid>, IFloorRepository
{
    public FloorRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Floor>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(f => f.Rooms)
            .Where(f => f.BuildingId == buildingId)
            .OrderBy(f => f.FloorNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> FloorNumberExistsInBuildingAsync(Guid buildingId, int floorNumber, Guid? excludeFloorId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(f => f.BuildingId == buildingId && f.FloorNumber == floorNumber);

        if (excludeFloorId.HasValue)
        {
            query = query.Where(f => f.Id != excludeFloorId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}