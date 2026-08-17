using MediatR;
using Nms.Application.Locations.Rooms.Dtos;

namespace Nms.Application.Locations.Rooms.Commands.UpdateRoom;

public record UpdateRoomCommand(Guid Id, UpdateRoomDto Dto) : IRequest<RoomDto>;