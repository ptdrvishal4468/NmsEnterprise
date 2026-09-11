using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class RoomRepository : GenericRepository<Room, Guid>, IRoomRepository
{
    public RoomRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Room>> GetByFloorIdAsync(Guid floorId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(r => r.Racks)
            .Where(r => r.FloorId == floorId)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsInFloorAsync(Guid floorId, string code, Guid? excludeRoomId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = DbSet.AsNoTracking().Where(r => r.FloorId == floorId && r.Code == normalizedCode);

        if (excludeRoomId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoomId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}