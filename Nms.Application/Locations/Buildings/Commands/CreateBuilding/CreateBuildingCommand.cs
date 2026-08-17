using MediatR;
using Nms.Application.Locations.Buildings.Dtos;

namespace Nms.Application.Locations.Buildings.Commands.CreateBuilding;

public record CreateBuildingCommand(CreateBuildingDto Dto) : IRequest<BuildingDto>;