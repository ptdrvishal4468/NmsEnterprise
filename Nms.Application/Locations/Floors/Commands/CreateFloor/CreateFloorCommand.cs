using MediatR;
using Nms.Application.Locations.Floors.Dtos;

namespace Nms.Application.Locations.Floors.Commands.CreateFloor;

public record CreateFloorCommand(CreateFloorDto Dto) : IRequest<FloorDto>;