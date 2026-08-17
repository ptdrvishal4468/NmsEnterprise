using MediatR;
using Nms.Application.Locations.Rooms.Dtos;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomsByFloor;

public record GetRoomsByFloorQuery(Guid FloorId) : IRequest<IReadOnlyList<RoomDto>>;