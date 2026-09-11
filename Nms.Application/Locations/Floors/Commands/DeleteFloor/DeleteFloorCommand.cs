using MediatR;

namespace Nms.Application.Locations.Floors.Commands.DeleteFloor;

public record DeleteFloorCommand(Guid Id) : IRequest<bool>;