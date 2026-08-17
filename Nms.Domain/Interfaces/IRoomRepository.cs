namespace Nms.Domain.Interfaces;

using Nms.Domain.Entities;

public interface IRoomRepository : IGenericRepository<Room, Guid>
{
    Task<IReadOnlyList<Room>> GetByFloorIdAsync(Guid floorId, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsInFloorAsync(Guid floorId, string code, Guid? excludeRoomId = null, CancellationToken cancellationToken = default);
}