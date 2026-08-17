using MediatR;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Queries.GetFloorById;

public class GetFloorByIdQueryHandler : IRequestHandler<GetFloorByIdQuery, FloorDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFloorByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FloorDto?> Handle(GetFloorByIdQuery request, CancellationToken cancellationToken)
    {
        var floor = await _unitOfWork.Floors.GetByIdAsync(request.Id, cancellationToken);
        if (floor == null)
        {
            return null;
        }

        return new FloorDto(
            floor.Id,
            floor.TenantId,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.Description,
            floor.IsActive,
            floor.CreatedAtUtc,
            floor.CreatedBy,
            floor.LastModifiedAtUtc,
            floor.LastModifiedBy,
            floor.Rooms != null ? floor.Rooms.Count : 0);
    }
}