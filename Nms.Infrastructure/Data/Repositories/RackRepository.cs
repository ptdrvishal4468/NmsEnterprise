using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class RackRepository : GenericRepository<Rack, Guid>, IRackRepository
{
    public RackRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Rack>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(rk => rk.RoomId == roomId)
            .OrderBy(rk => rk.Identifier)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IdentifierExistsInRoomAsync(Guid roomId, string identifier, Guid? excludeRackId = null, CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = identifier.Trim().ToUpperInvariant();
        var query = DbSet.AsNoTracking().Where(rk => rk.RoomId == roomId && rk.Identifier == normalizedIdentifier);

        if (excludeRackId.HasValue)
        {
            query = query.Where(rk => rk.Id != excludeRackId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}