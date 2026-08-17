using MediatR;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomsByFloor;

public class GetRoomsByFloorQueryHandler : IRequestHandler<GetRoomsByFloorQuery, IReadOnlyList<RoomDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoomsByFloorQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<RoomDto>> Handle(GetRoomsByFloorQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _unitOfWork.Rooms.GetByFloorIdAsync(request.FloorId, cancellationToken);

        return rooms.Select(r => new RoomDto(
            r.Id,
            r.TenantId,
            r.FloorId,
            r.Name,
            r.Code,
            r.RoomType,
            r.Description,
            r.IsActive,
            r.CreatedAtUtc,
            r.CreatedBy,
            r.LastModifiedAtUtc,
            r.LastModifiedBy,
            r.Racks != null ? r.Racks.Count : 0)).ToList();
    }
}