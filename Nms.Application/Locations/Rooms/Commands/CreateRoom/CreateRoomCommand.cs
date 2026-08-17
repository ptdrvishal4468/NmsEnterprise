using MediatR;
using Nms.Application.Locations.Rooms.Dtos;

namespace Nms.Application.Locations.Rooms.Commands.CreateRoom;

public record CreateRoomCommand(CreateRoomDto Dto) : IRequest<RoomDto>;