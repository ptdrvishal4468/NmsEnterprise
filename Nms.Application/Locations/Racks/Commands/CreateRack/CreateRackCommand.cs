using MediatR;
using Nms.Application.Locations.Racks.Dtos;

namespace Nms.Application.Locations.Racks.Commands.CreateRack;

public record CreateRackCommand(CreateRackDto Dto) : IRequest<RackDto>;