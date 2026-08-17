using MediatR;

namespace Nms.Application.Locations.Buildings.Commands.DeleteBuilding;

public record DeleteBuildingCommand(Guid Id) : IRequest<bool>;