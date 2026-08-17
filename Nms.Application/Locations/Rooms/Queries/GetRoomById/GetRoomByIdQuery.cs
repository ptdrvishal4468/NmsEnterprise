using MediatR;
using Nms.Application.Locations.Rooms.Dtos;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomById;

public record GetRoomByIdQuery(Guid Id) : IRequest<RoomDto?>;