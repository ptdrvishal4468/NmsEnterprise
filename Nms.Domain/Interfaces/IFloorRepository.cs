namespace Nms.Domain.Interfaces;

using Nms.Domain.Entities;

public interface IFloorRepository : IGenericRepository<Floor, Guid>
{
    Task<IReadOnlyList<Floor>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default);
    Task<bool> FloorNumberExistsInBuildingAsync(Guid buildingId, int floorNumber, Guid? excludeFloorId = null, CancellationToken cancellationToken = default);
}