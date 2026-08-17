using MediatR;
using Nms.Application.Locations.Racks.Dtos;

namespace Nms.Application.Locations.Racks.Queries.GetRacksByRoom;

public record GetRacksByRoomQuery(Guid RoomId) : IRequest<IReadOnlyList<RackDto>>;