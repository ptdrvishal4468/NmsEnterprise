using MediatR;
using Nms.Application.Locations.Floors.Dtos;

namespace Nms.Application.Locations.Floors.Commands.UpdateFloor;

public record UpdateFloorCommand(Guid Id, UpdateFloorDto Dto) : IRequest<FloorDto>;