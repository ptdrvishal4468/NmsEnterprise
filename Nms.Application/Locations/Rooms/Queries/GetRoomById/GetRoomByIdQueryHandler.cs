using MediatR;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, RoomDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoomByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomDto?> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(request.Id, cancellationToken);
        if (room == null)
        {
            return null;
        }

        return new RoomDto(
            room.Id,
            room.TenantId,
            room.FloorId,
            room.Name,
            room.Code,
            room.RoomType,
            room.Description,
            room.IsActive,
            room.CreatedAtUtc,
            room.CreatedBy,
            room.LastModifiedAtUtc,
            room.LastModifiedBy,
            room.Racks != null ? room.Racks.Count : 0);
    }
}