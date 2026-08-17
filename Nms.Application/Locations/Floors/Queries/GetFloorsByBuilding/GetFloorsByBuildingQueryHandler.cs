using MediatR;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Queries.GetFloorsByBuilding;

public class GetFloorsByBuildingQueryHandler : IRequestHandler<GetFloorsByBuildingQuery, IReadOnlyList<FloorDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFloorsByBuildingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<FloorDto>> Handle(GetFloorsByBuildingQuery request, CancellationToken cancellationToken)
    {
        var floors = await _unitOfWork.Floors.GetByBuildingIdAsync(request.BuildingId, cancellationToken);

        return floors.Select(f => new FloorDto(
            f.Id,
            f.TenantId,
            f.BuildingId,
            f.Name,
            f.FloorNumber,
            f.Description,
            f.IsActive,
            f.CreatedAtUtc,
            f.CreatedBy,
            f.LastModifiedAtUtc,
            f.LastModifiedBy,
            f.Rooms != null ? f.Rooms.Count : 0)).ToList();
    }
}