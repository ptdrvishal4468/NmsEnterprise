using MediatR;
using Nms.Application.Topology.Dtos;

namespace Nms.Application.Topology.Commands.DiscoverNeighbors;

public record DiscoverNeighborsCommand(Guid? SpecificDeviceId = null) : IRequest<DiscoverNeighborsResultDto>;