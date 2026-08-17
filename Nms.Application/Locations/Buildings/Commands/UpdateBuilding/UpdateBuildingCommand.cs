using MediatR;
using Nms.Application.Locations.Buildings.Dtos;

namespace Nms.Application.Locations.Buildings.Commands.UpdateBuilding;

public record UpdateBuildingCommand(Guid Id, UpdateBuildingDto Dto) : IRequest<BuildingDto>;