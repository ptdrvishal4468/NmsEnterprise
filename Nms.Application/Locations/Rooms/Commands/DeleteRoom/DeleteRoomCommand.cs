using MediatR;

namespace Nms.Application.Locations.Rooms.Commands.DeleteRoom;

public record DeleteRoomCommand(Guid Id) : IRequest<bool>;