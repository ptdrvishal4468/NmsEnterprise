namespace Nms.Domain.Interfaces;

using Nms.Domain.Entities;

public interface IRackRepository : IGenericRepository<Rack, Guid>
{
    Task<IReadOnlyList<Rack>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<bool> IdentifierExistsInRoomAsync(Guid roomId, string identifier, Guid? excludeRackId = null, CancellationToken cancellationToken = default);
}